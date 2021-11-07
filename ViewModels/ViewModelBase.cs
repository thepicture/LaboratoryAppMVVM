using LaboratoryAppMVVM.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LaboratoryAppMVVM.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private string _title = "";
        private IMessageBoxService _messageBoxService;

        public string Title
        {
            get => _title; set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public IMessageBoxService MessageBoxService
        {
            get => _messageBoxService; set
            {
                _messageBoxService = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
