
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AccountantViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _viewModelNavigationStore;

        public AccountantViewModel(ViewModelNavigationStore viewModelNavigationStore)
        {
            _viewModelNavigationStore = viewModelNavigationStore;
        }
    }
}
