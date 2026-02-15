EDCRNumber_DataCard3

EngLocation_DataCard1
Required - True

RequestType_DataCard3
Required - true
Update - //DataCardValue33.Selected
{
    Id: DataCardValue33.Selected.ID,
    Value: DataCardValue33.Selected.'Request Type'
}

RequestDate_DataCard1
Required - true

DocumentNumber_DataCard2

ReviewByDate_DataCard1
Required - true

DataCard2

RequestedBy_DataCard1
Required - true

ProductLine_DataCard3
Required - true
Update - //DataCardValue35.Selected
{
    Id: DataCardValue35.Selected.ID,
    Value: DataCardValue35.Selected.'Product Line'
}

ProductLineMgr_DataCard1
Required - true

Reviewing Manager_DataCard1
Required - true

Description_DataCard3
Justification_DataCard2
FormStatus_DataCard2
Default - //ThisItem.FormStatus
If(
    Form1.Mode = FormMode.New,
    { Value: "Active" },
    ThisItem.FormStatus
)
DisplayMode - DisplayMode.View

Attachments_DataCard2
Height - 70