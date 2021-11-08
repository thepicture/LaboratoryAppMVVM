
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private ViewModelNavigationStore navigationStore;

        public AdminViewModel(ViewModelNavigationStore navigationStore, Models.Entities.User user)
        {
            this.navigationStore = navigationStore;
        }
    }
}
