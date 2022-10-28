namespace tl121pet.Storage
{
    public static class FormThemeDict
    {
        public static Dictionary<FormMode, string> ThemeDict = new Dictionary<FormMode, string>() {
            { FormMode.Edit, "warning" },
            { FormMode.Create, "success" },
            { FormMode.Details, "primary" },
            { FormMode.Process, "success" }
        };
    }
}
