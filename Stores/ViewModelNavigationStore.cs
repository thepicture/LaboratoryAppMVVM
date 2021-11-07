using LaboratoryAppMVVM.ViewModels;
using System;

namespace LaboratoryAppMVVM.Stores
{
    public class ViewModelNavigationStore
    {
        private ViewModelBase _currentViewModel;
        public event Action CurrentViewModelChanged;

        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel; set
            {
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
    }
}
