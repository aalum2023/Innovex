Set(
    varAppLink,
    varAppBaseUrl &
    "?mode=reviewmanager" &
    "&edcrid=" & DataCardValue26.Value
);

Set(varReviewManager, DataCardValue1.Value);
SubmitForm(Form3);
