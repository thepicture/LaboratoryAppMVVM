using System.Windows;

namespace LaboratoryAppMVVM.Services
{
    public class LaboratoryWindowService : IWindowService
    {
        public void ShowWindow(object viewModel)
        {
            Window window = new Window
            {
                Content = viewModel
            };
            window.Show();
        }
    }
}
