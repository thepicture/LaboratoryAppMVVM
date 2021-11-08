
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LaboratoryResearcherViewModel : ViewModelBase
    {
        private ViewModelNavigationStore navigationStore;

        public LaboratoryResearcherViewModel(ViewModelNavigationStore navigationStore, Models.Entities.User user)
        {
            this.navigationStore = navigationStore;
        }
    }
}
