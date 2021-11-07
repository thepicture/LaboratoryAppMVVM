namespace LaboratoryAppMVVM.Models
{
    public interface IMessageBoxService
    {
        bool ShowQuestion(string message);
        void ShowError(string message);
        void ShowInformation(string message);
    }
}
