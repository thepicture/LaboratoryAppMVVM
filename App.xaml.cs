using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System.Windows;

namespace LaboratoryAppMVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ViewModelNavigationStore navigationStore = new ViewModelNavigationStore();
            ILoginService<TypeOfUser, ViewModelNavigationStore> loginService = new LaboratoryLoginService();
            MessageBoxService messageBoxService = new MessageBoxService();
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore, messageBoxService, loginService);
            MainView mainView = new MainView
            {
                DataContext = new MainViewModel(navigationStore, messageBoxService)
            };
            mainView.Show();

            base.OnStartup(e);
        }
    }
}
