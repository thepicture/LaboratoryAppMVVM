using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        protected ViewModelNavigationStore _viewModelNavigationStore;
        public ViewModelBase CurrentViewModel => _viewModelNavigationStore.CurrentViewModel;
        public MainViewModel(ViewModelNavigationStore viewModelNavigationStore, IMessageBoxService messageBoxService)
        {
            MessageBoxService = messageBoxService;
            _viewModelNavigationStore = viewModelNavigationStore;
            viewModelNavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

    }
}
