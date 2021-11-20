using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LaboratoryAppMVVM.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        private string _title = "";
        private IMessageService _messageBoxService;


        public string Title
        {
            get => _title; set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        public IMessageService MessageBoxService
        {
            get => _messageBoxService; set
            {
                _messageBoxService = value;
                OnPropertyChanged();
            }
        }

        public User User { get; protected set; } = new User();

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
