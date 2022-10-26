using tl121pet.Entities.Models;
using tl121pet.Storage;

namespace tl121pet.ViewModels
{
    public class PersonVM
    {
        public Person SelectedPerson { get; set; } = new Person();
        public FormMode Mode { get; set; }

        public string Theme => themeDict[Mode];
        public bool IsReadonly => Mode == FormMode.Details ? true : false;

        public string ReadonlyTag => IsReadonly ? "readonly" : "";

        private Dictionary<FormMode, string> themeDict = new Dictionary<FormMode, string>() {
            { FormMode.Edit, "warning" },
            { FormMode.Create, "success" },
            { FormMode.Details, "primary" }
        };
    }
}
