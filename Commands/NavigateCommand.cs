using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System;

namespace LaboratoryAppMVVM.Commands
{
    public class NavigateCommand<TViewModel> where TViewModel : ViewModelBase
    {
        public NavigateCommand(ViewModelNavigationStore navigationStore, Func<TViewModel> createViewModelFunc)
        {
            navigationStore.CurrentViewModel = createViewModelFunc();
        }
    }
}
