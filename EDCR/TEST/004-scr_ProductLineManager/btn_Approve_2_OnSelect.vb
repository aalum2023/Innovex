/*SubmitForm(Form2);
ResetForm(Form2);
NewForm(Form2);*/
/* ===================== SET VARIABLES ===================== */
Set(
    varAppLink,
    varAppBaseUrl & "?mode=edit" & "&edcrid=" & DataCardValue26.Value
);
Set(
    varAppLink_engineeringassigneeApproval,
    varAppBaseUrl & "?mode=engineeringassignee" & "&edcrid=" & DataCardValue26.Value
);
Set(
    varEDCRItem,
    LookUp(
        EDCRRequest,
        EDCRNumber = varEDCRID
    )
);
/* ===================== VALIDATION ===================== */
If(
    IsBlank(DataCardValue17.Value) || IsBlank(DataCardValue15.Selected) || IsBlank(DataCardValue18.Selected) || IsBlank(DataCardValue16.SelectedDate),
    Notify(
        "⚠️ Please fill all mandatory fields before submitting.",
        NotificationType.Warning,
        5000
    ),
    /* ===================== PATCH ===================== */
    IfError(
        Patch(
            ProductLineManager,
            Defaults(ProductLineManager),
            {
                EDCRNumber: {
                    Id: varEDCRItem.ID,
                    Value: varEDCRItem.EDCRNumber
                },
                Comments: DataCardValue17.Value,
                AssignedTo: DataCardValue15.Selected.Email,
                Deadline: DataCardValue16.SelectedDate,
                EngineeringStatus: DataCardValue18.Selected,
                EngineeringManager: DataCardValue19.Value,
                Signed: DataCardValue20.Selected
            }
        );Patch(EDCRRequest,First(Filter(EDCRRequest,EDCRNumber=varEDCRID)),{FormStatus:If(DataCardValue18.Selected.Value="Reject",Dropdown1_4.Selected,Dropdown1_1.Selected)}),
        Notify(
            "❌ Unable to save review details. Please try again.",
            NotificationType.Error,
            6000
        ),
        /* ===================== EMAIL LOGIC ===================== */
        Switch(
            DataCardValue18.Selected.Value,
            /* ===================== APPROVE ===================== */
            "Approve",
            Concurrent(
                /* ---- Mail 1 to Requestor ---- */
                Office365Outlook.SendEmail(
                    DataCardValue3.Value,
                    varFlag&" - Engineering EDCR – Approved",
                    Concatenate(
                        "<div style='font-family:Segoe UI;max-width:650px;margin:auto;border:2px solid #5E35B1;'>",
                        "<div style='background:#5E35B1;color:white;padding:20px;text-align:center;'>",
                        "<h2>🟣 Engineering EDCR Approved</h2></div>",
                        "<div style='padding:20px;'>",
                        "<p>The EDCR <b>",
                        varEDCRID,
                        "</b> has been <b style='color:#5E35B1;'>approved</b>.</p>",
                        "<p><b>Reviewed By:</b> ",
                        DataCardValue9.Value,
                        "</p>",
                        "<p><b>Review Date:</b> ",
                        Text(
                            DataCardValue10.SelectedDate,
                            "dd-mm-yyyy"
                        ),
                        "</p>",
                        "<p>Please proceed with the next steps.</p>",
                        "</div></div>"
                    ),
                    {IsHtml: true}
                ),
                /* ---- Mail 2 to Product Line Manager ---- */
                Office365Outlook.SendEmail(
                    varRequestInfo.ProductLineMgr,
                    varFlag&" - Engineering EDCR – Product Line Approval Required",
                    Concatenate(
                        "<div style='font-family:Segoe UI; max-width:650px; margin:auto; border-radius:12px; overflow:hidden;",
                        "box-shadow:0 8px 20px rgba(0,0,0,0.25); border:2px solid #5E35B1;'>",
                        "<div style='background-color:#5E35B1; color:#F3E5F5; padding:25px; text-align:center;'>",
                        "<h1 style='margin:0; font-size:26px;'>🟣 Engineering EDCR – Approval Required</h1>",
                        "</div>",
                        "<div style='padding:25px; background-color:#ffffff;'>",
                        "<p>Dear Product Line Manager,</p>",
                        "<p>The <strong>Engineering Document Change Request (EDCR)</strong>",
                        " <b>",
                        varEDCRID,
                        "</b> has been <strong>approved by Engineering</strong>.</p>",
                        "<div style='border-left:6px solid #7E57C2; background-color:#EDE7F6; padding:20px;",
                        "border-radius:8px; margin:20px 0;'>",
                        "<h3 style='color:#5E35B1;'>📋 Approval Details</h3>",
                        "<p><strong>Reviewed By:</strong> ",
                        DataCardValue9.Value,
                        "</p>",
                        "<p><strong>Review Date:</strong> ",
                        Text(
                            DataCardValue10.SelectedDate,
                            "dd-mm-yyyy"
                        ),
                        "</p>",
                        "<p><strong>Status:</strong> <span style='color:#FF8F00;font-weight:bold;'>Pending Engineering Assignee Approval</span></p>",
                        "</div>",
                        "<div style='text-align:center; margin:25px 0;'>",
                        "<a href='",
                        varAppLink_engineeringassigneeApproval,
                        "' style='background-color:#FFB300;color:#000;",
                        "padding:16px 35px;text-decoration:none;border-radius:10px;font-size:17px;",
                        "font-weight:bold;display:inline-block;'>Review & Take Action</a>",
                        "</div>",
                        "<p>Thank you,<br><strong>Engineering EDCR System</strong></p>",
                        "</div>",
                        "<div style='background-color:#f5f5f5; text-align:center; padding:15px;",
                        "font-size:12px; color:#666;'>Automated email – do not reply.</div>",
                        "</div>"
                    ),
                    {IsHtml: true}
                )
            ),
            /* ===================== NEEDS MORE INFORMATION ===================== */
            "Needs more information",
            Office365Outlook.SendEmail(
                DataCardValue3.Value,
                varFlag&" - Engineering EDCR – More Information Required",
                Concatenate(
                    "<div style='font-family:Segoe UI;max-width:650px;margin:auto;border:2px solid #F9A825;'>",
                    "<div style='background:#F9A825;color:white;padding:20px;text-align:center;'>",
                    "<h2>🟡 More Information Required</h2></div>",
                    "<div style='padding:20px;'>",
                    "<p>Your EDCR <b>",
                    varEDCRID,
                    "</b> requires additional information.</p>",
                    "<div style='background:#FFFDE7;padding:15px;border-left:6px solid #F9A825;'>",
                    "<p><b>Comments:</b><br>",
                    DataCardValue6.Value,
                    "</p></div>",
                    "<div style='text-align:center;margin:30px 0;'>",
                    "<a href='",
                    varAppLink,
                    "' style='background:#F9A825;color:white;",
                    "padding:14px 28px;text-decoration:none;border-radius:8px;font-weight:bold;'>Resubmit EDCR</a>",
                    "</div>",
                    "</div></div>"
                ),
                {IsHtml: true}
            ),
            /* ===================== REJECT ===================== */
            "Reject",
            Office365Outlook.SendEmail(
                DataCardValue3.Value,
                varFlag&" - Engineering EDCR – Rejected",
                Concatenate(
                    "<div style='font-family:Segoe UI;max-width:650px;margin:auto;border:2px solid #D32F2F;'>",
                    "<div style='background:#D32F2F;color:white;padding:20px;text-align:center;'>",
                    "<h2>❌ Engineering EDCR Rejected</h2></div>",
                    "<div style='padding:20px;'>",
                    "<p>Your EDCR <b>",
                    varEDCRID,
                    "</b> has been rejected.</p>",
                    "<div style='background:#FDECEA;padding:15px;border-left:6px solid #D32F2F;'>",
                    "<p><b>Remarks:</b><br>",
                    DataCardValue6.Value,
                    "</p></div>",
                    "</div></div>"
                ),
                {IsHtml: true}
            )
        ),
        /* ===================== FINAL SUCCESS ===================== */
        Notify(
            "✔️ Engineering review completed and notifications sent.",
            NotificationType.Success,
            5000
        );
        ResetForm(Form2);
        NewForm(Form2)
    )
);
Navigate(Success_Approval);