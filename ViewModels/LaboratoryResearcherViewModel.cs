
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LaboratoryResearcherViewModel : ViewModelBase
    {
        private ViewModelNavigationStore navigationStore;

        public LaboratoryResearcherViewModel(ViewModelNavigationStore navigationStore)
        {
            this.navigationStore = navigationStore;
        }
    }
}
