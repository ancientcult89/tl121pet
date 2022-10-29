using tl121pet.Storage;

namespace tl121pet.ViewModels
{
    public class SimpleEditFormVM<T>
    {
        public T SelectedItem { get; set; }
        public FormMode Mode { get; set; }

        public string Theme => FormThemeDict.ThemeDict[Mode];
        public bool IsReadonly => Mode == FormMode.Details ? true : false;

        public string ReadonlyTag => IsReadonly ? "readonly" : "";
        public bool IsNewRecord { get; set; } = true;
    }
}
