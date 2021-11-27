using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exports;
using LaboratoryAppMVVM.Modelss.Exports;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AppliedServiceReportViewModel : ViewModelBase
    {
        private const string correctPeriodMessage = "Укажите корректный период выше";
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly AdminViewModel _adminViewModel;
        private readonly LaboratoryDatabaseEntities _context;
        private string _validationErrors = correctPeriodMessage;
        private TableOrChartExporter _exporter;

        public AppliedServiceReportViewModel(ViewModelNavigationStore navigationStore,
                                             AdminViewModel adminViewModel,
                                             MessageBoxService messageBoxService,
                                             LaboratoryDatabaseEntities context)
        {
            _navigationStore = navigationStore;
            _adminViewModel = adminViewModel;
            MessageService = messageBoxService;
            Title = "Отчёт по оказанным услугам";
            _dateTimeValidator = new DateTimeValidator();
            _context = context;
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
                    navigateToAdminViewModelCommand =
                        new RelayCommand(NavigateToAdminViewModel);
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

        private DateTime _fromPeriod = DateTime.Today;

        public DateTime FromPeriod
        {
            get => _fromPeriod; set
            {
                _fromPeriod = value;
                CheckIfPeriodIsCorrect();
                OnPropertyChanged();
            }
        }

        private DateTime _toPeriod = DateTime.Today;
        private readonly IValidator _dateTimeValidator;

        public DateTime ToPeriod
        {
            get => _toPeriod; set
            {
                _toPeriod = value;
                CheckIfPeriodIsCorrect();
                OnPropertyChanged();
            }
        }

        public string ValidationErrors
        {
            get => _validationErrors; set
            {
                _validationErrors = value;
                OnPropertyChanged();
            }
        }

        private void CheckIfPeriodIsCorrect()
        {
            bool IsCorrectPeriod = _dateTimeValidator.IsValidated(FromPeriod, ToPeriod);
            ValidationErrors = !IsCorrectPeriod
                ? correctPeriodMessage
                : string.Empty;
        }

        private RelayCommand generateReportCommand;
        private Chart _chart;
        private AppliedServiceReport _report;
        private string _selectedSavePath;

        public ICommand GenerateReportCommand
        {
            get
            {
                if (generateReportCommand == null)
                {
                    generateReportCommand = new RelayCommand(GenerateReport);
                }

                return generateReportCommand;
            }
        }

        private void GenerateReport(object commandParameter)
        {
            _report = new AppliedServiceReport(_context,
                _fromPeriod,
                _toPeriod);
            if (_report.GetAppliedServicesCount() == 0)
            {
                return;
            }
            _chart = new Chart();
            ChartArea chartArea = new ChartArea("ServiceArea");
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Angle = -45;

            _chart.ChartAreas.Add(chartArea);
            _chart.Legends.Add(new Legend());
            Series seriesPatientsPerDay = new Series("Количество пациентов " +
                "в день по каждой услуге")
            {
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Cross,
            };

            foreach (Tuple<Service, int> tuple in _report.GetPatientsPerDayOfServices())
            {
                _ = seriesPatientsPerDay.Points.AddXY(
                    tuple.Item1.Name,
                    tuple.Item2);
            }
            _chart.Series.Add(seriesPatientsPerDay);

            Series seriesMeanResultOfServices = new Series("Средний результат " +
                "каждого исследования в день по выбранному периоду")
            {
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Cross,
            };

            foreach (Tuple<Service, double> tuple
                in _report.GetMeanResultOfServicesPerPeriod())
            {
                _ = seriesPatientsPerDay.Points.AddXY(
                    tuple.Item1.Name,
                    tuple.Item2);
            }
            _chart.Series.Add(seriesMeanResultOfServices);
            ExportPresentation();
        }

        private void ExportPresentation()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            _selectedSavePath = folderBrowserDialog.SelectedPath;
            if (_chart == null)
            {
                ShowCantAccessChartError();
                return;
            }
            switch (CurrentExportType)
            {
                case "только график":
                    ExportChartToPdf();
                    break;
                case "только таблица":
                    ExportTableToPdf();
                    break;
                case "график и таблица":
                    ExportChartToPdf();
                    ExportTableToPdf();
                    break;
                default:
                    break;
            }
        }

        private void ShowCantAccessChartError()
        {
            MessageService.ShowError("Экспорт неуспешен, потому что "
                                     + "график недоступен. "
                                     + "Пожалуйста, перезайдите на страницу"
                                     + "и попробуйте ещё раз");
        }

        private void ExportTableToPdf()
        {
            throw new NotImplementedException();
        }

        private void ExportChartToPdf()
        {
            using (_exporter = new AppliedServiceTableOrChartPdfExporter(_report,
                 _selectedSavePath,
                 _chart))
            {
                _exporter.ExportAsChart();
            }
        }
    }
}
