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
            navigationStore.CurrentViewModel = new LoginViewModel(navigationStore);
            MainView mainView = new MainView
            {
                DataContext = new MainViewModel(navigationStore)
            };
            mainView.Show();

            base.OnStartup(e);
        }
    }
}
