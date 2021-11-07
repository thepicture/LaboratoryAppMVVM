using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;

        public LoginViewModel(ViewModelNavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            Title = "Авторизация";
        }
    }
}
