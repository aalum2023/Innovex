If(
    Form3.Mode = FormMode.New,
    "Submit",
    If(
        Form3.Mode = FormMode.Edit,
        "Resubmit",
        "Submit"     // fallback for View/other modes
    )
)
