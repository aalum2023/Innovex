Patch(EDCRRequest,First(Filter(EDCRRequest,EDCRNumber=varEDCRID)),{FormStatus:Dropdown1_2.Selected});
SubmitForm(Form4);
Navigate(Success_Approval);
ResetForm(Form4);
NewForm(Form4);