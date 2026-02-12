Patch(EngineeringAssignee,varAdminEAEdit,Form5_3.Updates);
Patch(ProductLineManager,varPLManagerEdit,Form7.Updates);
Patch(ReviewingManager,varReviewManagerEdit,Form6.Updates);
//Patch(EDCRRequest,VarItem,Form5.Updates);
Patch(
    EDCRRequest,
    VarItem,
    {
        Title: Form5.Updates.Title,
        EngLocation: {
            '@odata.type': "#Microsoft.Azure.Connectors.SharePoint.SPListExpandedReference",
            Id: LookUp(Choices([@EDCRRequest].EngLocation),Value = DataCardValue43.Selected.Value).Id,
            Value: DataCardValue43.Selected.Value
        }
    }
);
Patch(
    EDCRRequest,
    VarItem,
    {
        Title: Form5.Updates.Title,
        RequestType: {
            '@odata.type': "#Microsoft.Azure.Connectors.SharePoint.SPListExpandedReference",
            Id: LookUp(Choices(EDCRRequest.RequestType), Value = DataCardValue40.Selected.Value).Id,
            Value: DataCardValue40.Selected.Value
        }
    }
);
Patch(
    EDCRRequest,
    VarItem,
    {
        Title: Form5.Updates.Title,
        ProductLine: {
            '@odata.type': "#Microsoft.Azure.Connectors.SharePoint.SPListExpandedReference",
            Id: LookUp(Choices([@EDCRRequest].ProductLine),Value = DataCardValue44.Selected.Value).Id,
            Value: DataCardValue44.Selected.Value
        }
    }
);
If(!IsBlank(DataCardValue50.Value),Patch(EDCRComments,Defaults(EDCRComments),{EDCRNumber:VarItem.EDCRNumber,Comments:DataCardValue50.Value}));
If(!IsBlank(DataCardValue59.Value),Patch(EDCRComments,Defaults(EDCRComments),{EDCRNumber:VarItem.EDCRNumber,Comments:DataCardValue59.Value}));

ResetForm(Form5_3);ResetForm(Form7);ResetForm(Form6);ResetForm(Form5);Navigate(Success);


