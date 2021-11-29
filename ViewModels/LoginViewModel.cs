﻿using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Generators;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private const int prohibitedToLoginTimeout = 10;
        private const int minCaptchaLettersCount = 3;
        private const int maxCaptchaLettersCount = 4;
        private const int captchaWidth = 200;
        private const int captchaHeight = 40;
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly ILoginService<User, ViewModelNavigationStore> _loginService;
        private string _loginText = "chacking0";
        private string _passwordText = "123";
        private ICommand _authorizeCommand;
        private ICommand _exitAppCommand;
        private ICommand _regenerateCaptchaCommand;
        private ICommand _checkCaptchaCommand;
        private LaboratoryDatabaseEntities _context;
        private readonly ICaptchaService _captchaService;
        private List<ListViewCaptchaLetter> _captchaLetters;
        private bool _isCaptchaEnabled = false;
        private bool _isInterfaceNotBlocked = true;
        private RenderTargetBitmap _noiseImage;
        private readonly NoiseGenerator _noiseGenerator;
        private bool _isLoggingIn;
        public LoginViewModel(
            ViewModelNavigationStore navigationStore,
            IMessageService messageBoxService,
            ILoginService<User, ViewModelNavigationStore> loginService)
        {
            MessageService = messageBoxService;
            _navigationStore = navigationStore;
            _loginService = loginService;
            Title = "Авторизация";
            _captchaService = new SimpleCaptchaService();
            CaptchaLetters = _captchaService.GetCaptchaList(
                minCaptchaLettersCount,
                maxCaptchaLettersCount)
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

        public ICommand AuthorizeCommand
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

        public ICommand ExitAppCommand
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

        public ICommand RegenerateCaptchaCommand
        {
            get
            {
                if (_regenerateCaptchaCommand == null)
                {
                    _regenerateCaptchaCommand =
                        new RelayCommand(param =>
                        {
                            CaptchaLetters = _captchaService.GetCaptchaList(
                                minCaptchaLettersCount,
                                maxCaptchaLettersCount)
                            .Cast<ListViewCaptchaLetter>()
                            .ToList();
                            NoiseImage = _noiseGenerator
                            .Generate(new System.Windows.Size(
                                captchaWidth,
                                captchaHeight));
                        });
                }
                return _regenerateCaptchaCommand;
            }
        }
        public ICommand CheckCaptchaCommand
        {
            get
            {
                if (_checkCaptchaCommand == null)
                {
                    _checkCaptchaCommand = new RelayCommand(param =>
                    {
                        if ((param as string) == string.Join(
                            "",
                            _captchaLetters.Select(c => c.Letter)))
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
            MessageService.ShowError("Авторизация запрещена на "
                                        + prohibitedToLoginTimeout
                                        + " секунд");
            IsInterfaceNotBlocked = false;
            await Task.Delay(TimeSpan.FromSeconds(prohibitedToLoginTimeout));
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

        public bool IsLoggingIn
        {
            get => _isLoggingIn; set
            {
                _isLoggingIn = value;
                OnPropertyChanged();
            }
        }

        private void TryToExitApp()
        {
            if (MessageService.ShowQuestion("Вы действительно хотите "
                                               + "выйти из приложения?"))
            {
                App.Current.Shutdown();
            }
        }

        private async void TryToAuthorize()
        {
            IsLoggingIn = true;
            User currentUser = await Task.Run(() => Context.User.ToList()
            .FirstOrDefault(user => user.Login.ToLower()
                                              .Equals(LoginText.ToLower()) &&
                                    user.Password.Equals(PasswordText)));
            IsLoggingIn = false;
            if (currentUser != null)
            {
                MessageService
                    .ShowInformation($"Авторизация успешна. "
                                     + $"Добро пожаловать, "
                                     + $"{currentUser.TypeOfUser.Name} "
                                     + $"{currentUser.Name}!");
                _navigationStore.CurrentViewModel = _loginService
                    .LoginInAndGetLoginType(
                    currentUser,
                    _navigationStore)();
            }
            else
            {
                MessageService.ShowError("Неуспешная авторизация. " +
                    "Неверный логин и/или пароль. " +
                    "Пожалуйста, проверьте введённые данные " +
                    "и попробуйте авторизоваться ещё раз");
                IsCaptchaEnabled = true;
                RegenerateCaptchaCommand.Execute(null);
            }
        }
    }
}
