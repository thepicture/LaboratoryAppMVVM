using System.Windows;

namespace LaboratoryAppMVVM.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public void ShowError(string message)
        {
            _ = MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInformation(string message)
        {
            _ = MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public bool ShowQuestion(string message)
        {
            MessageBoxResult result = MessageBox.Show(message,
                                                      "Вопрос",
                                                      MessageBoxButton.YesNo,
                                                      MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
