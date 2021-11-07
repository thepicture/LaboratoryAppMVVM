using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System;
using System.Linq;
using System.Security;
using System.Threading.Tasks;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private string _loginText = string.Empty;
        private string _passwordText = string.Empty;
        private RelayCommand _authorizeCommand;
        private RelayCommand _exitAppCommand;
        private LaboratoryDatabaseEntities _context;

        public LoginViewModel(ViewModelNavigationStore navigationStore,
                              Models.MessageBoxService messageBoxService)
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
                    _authorizeCommand = new RelayCommand(param => TryToAuthorize());
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

        public LaboratoryDatabaseEntities Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new LaboratoryDatabaseEntities();
                }
                return _context;
            }
            set => _context = value;
        }

        private void TryToExitApp()
        {
            if (MessageBoxService.ShowQuestion("Вы действительно хотите "
                                               + "выйти из приложения?"))
            {
                App.Current.Shutdown();
            }
        }

        private async void TryToAuthorize()
        {
            User currentUser = await Task.Run(() => Context.User.ToList()
            .FirstOrDefault(user => user.Login.ToLower()
                                              .Equals(LoginText.ToLower()) &&
                                    user.Password.Equals(PasswordText)));
            if (currentUser != null)
            {
                MessageBoxService.ShowInformation($"Авторизация успешна. " +
                    $"Добро пожаловать, {currentUser.Name}!");
            }
            else
            {
                MessageBoxService.ShowError("Неверный логин и/или пароль. " +
                    "Пожалуйста, проверьте введённые данные " +
                    "и попробуйте авторизоваться ещё раз");
            }
        }
    }
}
