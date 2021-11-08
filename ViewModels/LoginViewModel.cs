using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System.Linq;
using System.Threading.Tasks;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly ILoginService<TypeOfUser, ViewModelNavigationStore> _loginService;
        private string _loginText = string.Empty;
        private string _passwordText = string.Empty;
        private RelayCommand _authorizeCommand;
        private RelayCommand _exitAppCommand;
        private LaboratoryDatabaseEntities _context;
        public LoginViewModel(ViewModelNavigationStore navigationStore,
                              MessageBoxService messageBoxService,
                              ILoginService<TypeOfUser, ViewModelNavigationStore> loginService)
        {
            MessageBoxService = messageBoxService;
            _navigationStore = navigationStore;
            _loginService = loginService;
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
                    $"Добро пожаловать, {currentUser.TypeOfUser.Name} {currentUser.Name}!");
                _navigationStore.CurrentViewModel = _loginService.LoginInAndGetLoginType(currentUser.TypeOfUser, _navigationStore)();
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
