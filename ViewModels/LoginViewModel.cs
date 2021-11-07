using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Stores;
using System;
using System.Security;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private string _loginText;
        private string _passwordText;
        private RelayCommand _authorizeCommand;
        private RelayCommand _exitAppCommand;

        public LoginViewModel(ViewModelNavigationStore navigationStore, Models.MessageBoxService messageBoxService)
        {
            MessageBoxService = messageBoxService;
            _navigationStore = navigationStore;
            Title = "Авторизация";
        }

        public string LoginText
        {
            get => _loginText; set
            {
                _loginText = value;
                OnPropertyChanged();
            }
        }
        public string PasswordText
        {
            get => _passwordText; set
            {
                _passwordText = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AuthorizeCommand
        {
            get
            {
                if (_authorizeCommand == null)
                {
                    _authorizeCommand = new RelayCommand(param => TryToAuthorize(param as SecureString));
                }
                return _authorizeCommand;
            }
        }

        public RelayCommand ExitAppCommand
        {
            get
            {
                if (_exitAppCommand == null)
                {
                    _exitAppCommand = new RelayCommand(param => TryToExitApp());
                }
                return _exitAppCommand;
            }
        }

        private void TryToExitApp()
        {
            if (MessageBoxService.ShowQuestion("Вы действительно хотите выйти из приложения?"))
            {
                App.Current.Shutdown();
            }
        }

        private void TryToAuthorize(SecureString password)
        {
            throw new NotImplementedException();
        }
    }
}
