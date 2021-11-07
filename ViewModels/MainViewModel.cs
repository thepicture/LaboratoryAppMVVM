using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Stores;

namespace LaboratoryAppMVVM.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _viewModelNavigationStore;

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

        public ViewModelBase CurrentViewModel => _viewModelNavigationStore.CurrentViewModel;
    }
}
