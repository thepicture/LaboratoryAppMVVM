using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        protected ViewModelNavigationStore _viewModelNavigationStore;
        private bool _isNotOnLoginPage;
        private RelayCommand _navigateToLoginPageCommand;
        public ViewModelBase CurrentViewModel => _viewModelNavigationStore.CurrentViewModel;

        public bool IsNotOnLoginPage
        {
            get => _isNotOnLoginPage; set
            {
                _isNotOnLoginPage = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateToLoginPageCommand
        {
            get
            {
                if (_navigateToLoginPageCommand == null)
                {
                    _navigateToLoginPageCommand =
                        new RelayCommand(param =>
                        {
                            if (MessageBoxService.ShowQuestion("Вы действительно хотите завершить сессию?"))
                            {

                                _viewModelNavigationStore.CurrentViewModel
                                                        = new LoginViewModel(_viewModelNavigationStore,
                                                                             MessageBoxService,
                                                                             new LaboratoryLoginService());
                            }
                        });
                }
                return _navigateToLoginPageCommand;
            }
        }

        public MainViewModel(ViewModelNavigationStore viewModelNavigationStore, IMessageService messageBoxService)
        {
            MessageBoxService = messageBoxService;
            _viewModelNavigationStore = viewModelNavigationStore;
            viewModelNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            IsNotOnLoginPage = !(CurrentViewModel is LoginViewModel);
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }
}
