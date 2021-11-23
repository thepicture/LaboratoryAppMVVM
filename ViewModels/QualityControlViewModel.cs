using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using System.Windows.Input;

namespace LaboratoryAppMVVM.ViewModels
{
    public class QualityControlViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly ViewModelBase _viewModelToGoBack;
        private ICollection<Service> _services;
        private Service _currentService;
        private ICollection<string> _exportTypes;
        private string _currentExportType;
        private ICommand _navigateToAdminViewModelCommand;
        private ICommand _exportCommand;
        private LaboratoryDatabaseEntities _context;
        private ICollection<KeyValuePair> _keyValues;
        private WindowsFormsHost _currentChart;

        public QualityControlViewModel(ViewModelNavigationStore navigationStore,
                                       ViewModelBase viewModelToGoBack,
                                       IMessageService messageBoxService)
        {
            _navigationStore = navigationStore;
            _viewModelToGoBack = viewModelToGoBack;
            MessageService = messageBoxService;
            Title = "Страница контроля качества";
        }

        public ICollection<Service> Services
        {
            get
            {
                if (_services == null)
                {
                    _services = Context.Service.ToList();
                    CurrentService = _services.FirstOrDefault();
                }
                return _services;
            }

            set
            {
                _services = value;
                OnPropertyChanged();
            }
        }
        public Service CurrentService
        {
            get => _currentService; set
            {
                _currentService = value;
                UpdateChart();
                OnPropertyChanged();
            }
        }

        private void UpdateChart()
        {
            Chart chart = new Chart();
            ChartArea chartArea = new ChartArea("ServiceArea");
            chart.ChartAreas.Add(chartArea);
            chart.Legends.Add(new Legend());
            Series series = new Series(CurrentService.Name);
            series.ChartType = SeriesChartType.Line;
            foreach(AppliedService appliedService in CurrentService.AppliedService)
            {
                series.Points.AddXY(appliedService.FinishedDateTime.ToString("yyyy-MM-dd hh:mm:ss"), appliedService.Result);
            }
            chart.Series.Add(series);
            CurrentChart.Child = chart;
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
        public ICommand NavigateToAdminViewModelCommand
        {
            get
            {
                if (_navigateToAdminViewModelCommand == null)
                {
                    _navigateToAdminViewModelCommand = new RelayCommand(param =>
                    {
                        _navigationStore.CurrentViewModel = _viewModelToGoBack;
                    });
                }
                return _navigateToAdminViewModelCommand;
            }
        }
        public LaboratoryDatabaseEntities Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new LaboratoryDatabaseEntities();
                }
                return _context;
            }
        }

        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand == null)
                {
                    _exportCommand = new RelayCommand(param => ExportChart());
                }
                return _exportCommand;
            }
        }

        public ICollection<KeyValuePair> KeyValues
        {
            get => _keyValues; set
            {
                _keyValues = value;
                OnPropertyChanged();
            }
        }

        public WindowsFormsHost CurrentChart
        {
            get
            {
                if (_currentChart == null)
                {
                    _currentChart = new WindowsFormsHost();
                }
                return _currentChart;
            }

            set
            {
                _currentChart = value;
                OnPropertyChanged();
            }
        }

        private void ExportChart()
        {

        }
    }
}
