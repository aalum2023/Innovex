/* ===================== SET VARIABLES ===================== */
Set(
    varAppLink,
    varAppBaseUrl & "?mode=edit" & "&edcrid=" & DataCardValue26.Value
);
Set(
    varAppLink_ProductManagerApproval,
    varAppBaseUrl & "?mode=productlinemanager" & "&edcrid=" & DataCardValue26.Value
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
    IsBlank(DataCardValue5.Value) || // Reviewing Manager
    IsBlank(DataCardValue8.Selected) || // Review Status
    IsBlank(DataCardValue9.Value) || // Reviewed By
    IsBlank(DataCardValue10.SelectedDate), // Review Date
    Notify(
        "⚠️ Please fill all mandatory fields before submitting.",
        NotificationType.Warning,
        5000
    ),
    
    /* ===================== PATCH ===================== */
    IfError(
        Patch(
            ReviewingManager,
            Defaults(ReviewingManager),
            {
                EDCRNumber: {
                    Id: varEDCRItem.ID,
                    Value: varEDCRItem.EDCRNumber
                },
                Comments: DataCardValue6.Value,
                ReviewingManager: DataCardValue5.Value,
                SuggestedDealine: DataCardValue7.SelectedDate,
                ReviewStatus: DataCardValue8.Selected,
                ReviewedBy: DataCardValue9.Value,
                ReviewDate: DataCardValue10.SelectedDate,
                Sign: DataCardValue42.Selected
            }
        );Patch(EDCRRequest,First(Filter(EDCRRequest,EDCRNumber=varEDCRID)),{FormStatus:If(DataCardValue8.Selected.Value="Reject",Dropdown1_3.Selected,Dropdown1.Selected)}),
        Notify(
            "❌ Unable to save review details. Please try again.",
            NotificationType.Error,
            6000
        ),

        /* ===================== EMAIL LOGIC ===================== */
        Switch(
            DataCardValue8.Selected.Value,
            
            /* ===================== APPROVE ===================== */
            "Approve",
            Concurrent(
                /* ---- Mail 1 to Requestor ---- */
                Office365Outlook.SendEmail(
                    DataCardValue3.Value,
                    varFlag&" - Review of - EDCR – Approved",
                    Concatenate(
                        "<div style='font-family:Segoe UI;max-width:650px;margin:auto;border:2px solid #0A5396;'>",
                        "<div style='background:#0A5396;color:white;padding:20px;text-align:center;'>",
                        "<h2>✅ Review of - EDCR Approved</h2></div>",
                        "<div style='padding:20px;'>",
                        "<p>The EDCR <b>", varEDCRID,
                        "</b> has been <b style='color:#1E88E5;'>approved</b>.</p>",
                        "<p><b>Reviewed By:</b> ", DataCardValue9.Value, "</p>",
                        "<p><b>Review Date:</b> ", Text(DataCardValue10.SelectedDate, "dd-mm-yyyy"), "</p>",
                        "<p>Please proceed with the next steps.</p>",
                        "</div></div>"
                    ),
                    { IsHtml: true }
                ),

                /* ---- Mail 2 to Product Line Manager ---- */
                Office365Outlook.SendEmail(
                    varRequestInfo.ProductLineMgr,
                    varFlag&" - Review of EDCR – Product Line Approval Required",
                    Concatenate(
                        "<div style='font-family:Segoe UI; max-width:650px; margin:auto; border-radius:12px; overflow:hidden;",
                        "box-shadow:0 8px 20px rgba(0,0,0,0.25); border:2px solid #2E7D32;'>",
                        "<div style='background-color:#2E7D32; color:#E8F5E9; padding:25px; text-align:center;'>",
                        "<h1 style='margin:0; font-size:26px;'>🟢 EDCR – Approval Required</h1>",
                        "</div>",
                        "<div style='padding:25px; background-color:#ffffff;'>",
                        "<p style='font-size:15px;'>Dear Product Line Manager,</p>",
                        "<p style='font-size:15px;'>The <strong>Engineering Document Change Request (EDCR)</strong>",
                        " <b>", varEDCRID, "</b> has been <strong>approved by the Reviewing Manager</strong>.</p>",
                        "<p style='font-size:15px;'>It is now awaiting your decision as the <strong>Product Line Manager</strong>.</p>",
                        "<div style='border-left:6px solid #43A047; background-color:#E8F5E9; padding:20px;",
                        "border-radius:8px; margin:20px 0;'>",
                        "<h3 style='color:#2E7D32;'>📋 Approval Details</h3>",
                        "<p><strong>Reviewed By:</strong> ", DataCardValue9.Value, "</p>",
                        "<p><strong>Review Date:</strong> ", Text(DataCardValue10.SelectedDate, "dd-mm-yyyy"), "</p>",
                        "<p><strong>Current Status:</strong> <span style='color:#F57C00;font-weight:bold;'>Pending Product Line Approval</span></p>",
                        "</div>",
                        "<div style='text-align:center; margin:25px 0;'>",
                        "<a href='", varAppLink_ProductManagerApproval, "' style='background-color:#F57C00;color:white;font-weight:bold;",
                        "padding:16px 35px;text-decoration:none;border-radius:10px;font-size:17px;",
                        "display:inline-block;box-shadow:0 6px 12px rgba(0,0,0,0.3);'>Review & Take Action</a>",
                        "</div>",
                        "<p style='font-size:15px;'>Please review the EDCR and choose to <strong>Approve</strong> or <strong>Reject</strong>.</p>",
                        "<p style='font-size:15px;'>Thank you,<br><strong>EDCR System Notification</strong></p>",
                        "</div>",
                        "<div style='background-color:#f7f7f7; text-align:center; padding:15px;",
                        "font-size:12px; color:#666;'>This is an automated notification from the EDCR system. Please do not reply.</div>",
                        "</div>"
                    ),
                    { IsHtml: true }
                )
            ),

            /* ===================== NEEDS MORE INFORMATION ===================== */
            "Needs more information",
            Office365Outlook.SendEmail(
                DataCardValue3.Value,
                varFlag&" - Review of EDCR – More Information Required",
                Concatenate(
                    "<div style='font-family:Segoe UI;max-width:650px;margin:auto;border:2px solid #FB8C00;'>",
                    "<div style='background:#FB8C00;color:white;padding:20px;text-align:center;'>",
                    "<h2>🟠 More Information Required</h2></div>",
                    "<div style='padding:20px;'>",
                    "<p>Your EDCR <b>", varEDCRID, "</b> has been reviewed.</p>",
                    "<p><b>The Reviewing Manager has requested additional information.</b></p>",
                    "<div style='background:#FFF3E0;padding:15px;border-left:6px solid #FB8C00;margin:15px 0;'>",
                    "<p><b>Reviewer Comments:</b><br>", DataCardValue6.Value, "</p></div>",
                    "<p>Please update the EDCR with the required details and resubmit it for review.</p>",
                    "<div style='text-align:center;margin:30px 0;'>",
                    "<a href='", varAppLink, "' style='background:#FB8C00;color:white;font-weight:bold;",
                    "padding:14px 28px;text-decoration:none;border-radius:8px;display:inline-block;font-size:16px;'>Resubmit EDCR</a>",
                    "</div>",
                    "<p style='font-size:13px;color:#666;text-align:center;'>Click the button above to open the EDCR form, update the information, and resubmit it for review.</p>",
                    "<p>Thank you for your cooperation.</p>",
                    "</div></div>"
                ),
                { IsHtml: true }
            ),

            /* ===================== REJECT ===================== */
            "Reject",
            Office365Outlook.SendEmail(
                DataCardValue3.Value,
                varFlag&" - Review of EDCR – Rejected",
                Concatenate(
                    "<div style='font-family:Segoe UI;max-width:650px;margin:auto;border:2px solid #C62828;'>",
                    "<div style='background:#C62828;color:white;padding:20px;text-align:center;'>",
                    "<h2>❌ EDCR Rejected</h2></div>",
                    "<div style='padding:20px;'>",
                    "<p>After careful evaluation, your EDCR <b>", varEDCRID, "</b> has been <b style='color:#C62828;'>rejected</b>.</p>",
                    "<p><b>Reviewed By:</b> ", DataCardValue9.Value, "</p>",
                    "<div style='background:#FDECEA;padding:15px;border-left:6px solid #C62828;'>",
                    "<p><b>Reviewer Remarks:</b><br>", DataCardValue6.Value, "</p></div>",
                    "<p>If you need further clarification, please contact the Reviewing Manager.</p>",
                    "<p>Thank you for your submission.</p>",
                    "</div></div>"
                ),
                { IsHtml: true }
            )
        ),

        /* ===================== FINAL SUCCESS NOTIFY ===================== */
        Notify(
            "✔️ Review details saved successfully and email notifications sent.",
            NotificationType.Success,
            5000
        );
        ResetForm(Form3);
        NewForm(Form3)
    )
);
Navigate(Success_Approval);
