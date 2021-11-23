using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System.Windows.Input;

namespace LaboratoryAppMVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        protected ViewModelNavigationStore _viewModelNavigationStore;
        private bool _isNotOnLoginPage;
        private ICommand _navigateToLoginPageCommand;
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _viewModelNavigationStore.CurrentViewModel;
            }
        }

        public bool IsNotOnLoginPage
        {
            get => _isNotOnLoginPage; set
            {
                _isNotOnLoginPage = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToLoginPageCommand
        {
            get
            {
                if (_navigateToLoginPageCommand == null)
                {
                    _navigateToLoginPageCommand =
                        new RelayCommand(param => NavigateToLoginPage());
                }
                return _navigateToLoginPageCommand;
            }
        }

        private void NavigateToLoginPage()
        {

            if (IsUserValidatesQuestion())
            {
                _viewModelNavigationStore
                .CurrentViewModel =
                new LoginViewModel(_viewModelNavigationStore,
                    MessageService,
                    new LaboratoryLoginService());
            }
        }

        private bool IsUserValidatesQuestion()
        {
            return MessageService
                        .ShowQuestion("Вы действительно " +
                        "хотите завершить сессию?");
        }

        public MainViewModel(ViewModelNavigationStore viewModelNavigationStore,
                             IMessageService messageBoxService)
        {
            MessageService = messageBoxService;
            _viewModelNavigationStore = viewModelNavigationStore;
            viewModelNavigationStore
                .CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            IsNotOnLoginPage = !(CurrentViewModel is LoginViewModel);
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }
}
