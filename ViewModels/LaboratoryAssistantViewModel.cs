
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LaboratoryAssistantViewModel : ViewModelBase
    {
        private ViewModelNavigationStore navigationStore;

        public LaboratoryAssistantViewModel(ViewModelNavigationStore navigationStore)
        {
            this.navigationStore = navigationStore;
        }
    }
}
