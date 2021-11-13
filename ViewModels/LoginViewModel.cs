using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly ILoginService<User, ViewModelNavigationStore> _loginService;
        private string _loginText = "chacking0";
        private string _passwordText = "4tzqHdkqzo4";
        private RelayCommand _authorizeCommand;
        private RelayCommand _exitAppCommand;
        private RelayCommand _regenerateCaptchaCommand;
        private RelayCommand _checkCaptchaCommand;
        private LaboratoryDatabaseEntities _context;
        private readonly ICaptchaService _captchaService;
        private List<ListViewCaptchaLetter> _captchaLetters;
        private bool _isCaptchaEnabled = false;
        private bool _isInterfaceNotBlocked = true;
        private RenderTargetBitmap _noiseImage;
        private readonly NoiseGenerator _noiseGenerator;
        public LoginViewModel(ViewModelNavigationStore navigationStore,
                              IMessageBoxService messageBoxService,
                              ILoginService<User, ViewModelNavigationStore> loginService)
        {
            MessageBoxService = messageBoxService;
            _navigationStore = navigationStore;
            _loginService = loginService;
            Title = "Авторизация";
            _captchaService = new CaptchaWithLineService();
            CaptchaLetters = _captchaService.GetCaptchaList(3, 4)
                 .Cast<ListViewCaptchaLetter>()
                 .ToList();
            _noiseGenerator = new NoiseGenerator();
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

        public List<ListViewCaptchaLetter> CaptchaLetters
        {
            get => _captchaLetters; set
            {
                _captchaLetters = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand RegenerateCaptchaCommand
        {
            get
            {
                if (_regenerateCaptchaCommand == null)
                {
                    _regenerateCaptchaCommand =
                        new RelayCommand(param =>
                        {
                            CaptchaLetters = _captchaService.GetCaptchaList(3, 4)
                            .Cast<ListViewCaptchaLetter>()
                            .ToList();
                            NoiseImage = _noiseGenerator.Generate(200, 40);
                        });
                }
                return _regenerateCaptchaCommand;
            }
        }
        public RelayCommand CheckCaptchaCommand
        {
            get
            {
                if (_checkCaptchaCommand == null)
                {
                    _checkCaptchaCommand = new RelayCommand(param =>
                    {
                        if ((param as string) == string.Join("", _captchaLetters.Select(c => c.Letter)))
                        {
                            IsCaptchaEnabled = false;
                        }
                        else
                        {
                            BlockSystemInput();
                        }
                    });
                }
                return _checkCaptchaCommand;
            }
        }

        private async void BlockSystemInput()
        {
            MessageBoxService.ShowError("Вам запрещён вход на 10 секунд");
            IsInterfaceNotBlocked = false;
            await Task.Delay(TimeSpan.FromSeconds(10));
            IsInterfaceNotBlocked = true;
        }

        public bool IsCaptchaEnabled
        {
            get => _isCaptchaEnabled; set
            {
                _isCaptchaEnabled = value;
                OnPropertyChanged();
            }
        }

        public bool IsInterfaceNotBlocked
        {
            get => _isInterfaceNotBlocked; set
            {
                _isInterfaceNotBlocked = value;
                OnPropertyChanged();
            }
        }

        public RenderTargetBitmap NoiseImage
        {
            get => _noiseImage;

            set
            {
                _noiseImage = value;
                OnPropertyChanged();
            }
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
                _navigationStore.CurrentViewModel = _loginService.LoginInAndGetLoginType(currentUser, _navigationStore)();
            }
            else
            {
                MessageBoxService.ShowError("Неуспешная авторизация. " +
                    "Неверный логин и/или пароль. " +
                    "Пожалуйста, проверьте введённые данные " +
                    "и попробуйте авторизоваться ещё раз");
                IsCaptchaEnabled = true;
                RegenerateCaptchaCommand.Execute();
            }
        }
    }
}
