using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AppliedServiceReportViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly AdminViewModel _adminViewModel;
        private readonly MessageBoxService _messageBoxService;

        public AppliedServiceReportViewModel(ViewModelNavigationStore navigationStore,
                                             AdminViewModel adminViewModel,
                                             MessageBoxService messageBoxService)
        {
            _navigationStore = navigationStore;
            _adminViewModel = adminViewModel;
            _messageBoxService = messageBoxService;
            Title = "Отчёт по оказанным услугам";
        }

        private RelayCommand navigateToAdminViewModelCommand;
        private ICollection<string> _exportTypes;
        private string _currentExportType;

        public ICommand NavigateToAdminViewModelCommand
        {
            get
            {
                if (navigateToAdminViewModelCommand == null)
                {
                    navigateToAdminViewModelCommand = new RelayCommand(NavigateToAdminViewModel);
                }

                return navigateToAdminViewModelCommand;
            }
        }

        private void NavigateToAdminViewModel(object commandParameter)
        {
            _navigationStore.CurrentViewModel = _adminViewModel;
        }

        public ICollection<string> ExportTypes
        {
            get
            {
                if (_exportTypes == null)
                {
                    _exportTypes = new List<string>
                    {
                        "только график",
                        "только таблица",
                        "график и таблица"
                    };
                    CurrentExportType = _exportTypes.First();
                }
                return _exportTypes;
            }

            set
            {
                _exportTypes = value;
                OnPropertyChanged();
            }
        }
        public string CurrentExportType
        {
            get => _currentExportType; set
            {
                _currentExportType = value;
                OnPropertyChanged();
            }
        }

        private System.DateTime? _fromPeriod = System.DateTime.Today;

        public System.DateTime? FromPeriod
        {
            get => _fromPeriod; set
            {
                _fromPeriod = value;
                OnPropertyChanged();
            }
        }

        private System.DateTime? _toPeriod = System.DateTime.Today;

        public System.DateTime? ToPeriod
        {
            get => _toPeriod; set
            {
                _toPeriod = value;
                OnPropertyChanged();
            }
        }
    }
}
