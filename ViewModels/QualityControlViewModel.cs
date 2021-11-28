using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exports;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
        private WindowsFormsHost _windowsFormsHost;
        private double _meanDeviation = 0;
        private double _variationCoefficient = 0;
        private string _currentRepresentationForm;
        private ICollection<string> _representationForms;
        private bool _isChartForm = true;
        private Chart chart;
        private QualityControlReport _qualityControl;
        private string _selectedSavePath;

        public QualityControlViewModel(ViewModelNavigationStore navigationStore,
                                       ViewModelBase viewModelToGoBack,
                                       IMessageService messageService)
        {
            _navigationStore = navigationStore;
            _viewModelToGoBack = viewModelToGoBack;
            MessageService = messageService;
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
                if (_currentService.AppliedService.Count != 0)
                {
                    CurrentQualityControl = new QualityControlReport(_currentService);
                    MeanDeviation = CurrentQualityControl.GetMeanQuadrantDeviation();
                    VariationCoefficient = CurrentQualityControl
                        .GetVariationCoefficient();
                }
                else
                {
                    MeanDeviation = 0;
                    VariationCoefficient = 0;
                }
                OnPropertyChanged();
            }
        }

        private void UpdateChart()
        {
            if (CurrentService.AppliedService.Count == 0)
            {
                return;
            }
            CurrentQualityControl = new QualityControlReport(CurrentService);

            Chart = new Chart();
            ChartArea chartArea = new ChartArea("ServiceArea");
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Angle = -45;
            chartArea.AxisX.Interval = 3;

            _ = new Axis(chartArea, AxisName.Y2);
            Chart.ChartAreas.Add(chartArea);
            Chart.Legends.Add(new Legend());
            Series seriesLimit = new Series(CurrentService.Name)
            {
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Cross,
                YAxisType = AxisType.Primary
            };

            chartArea.AxisY2.Enabled = AxisEnabled.True;

            chartArea.AxisY2.Minimum =
                chartArea.AxisY.Minimum = CurrentQualityControl.GetStatisticLimit(-3);
            chartArea.AxisY2.Maximum =
                chartArea.AxisY.Maximum = CurrentQualityControl.GetStatisticLimit(4);
            chartArea.AxisY2.Interval =
                chartArea.AxisY.Interval =
                CurrentQualityControl.GetMeanQuadrantDeviation();
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(3),
                CurrentQualityControl.GetStatisticLimit(4),
                "+3s",
                0,
                LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(2),
                CurrentQualityControl.GetStatisticLimit(3),
                "+2s",
                0,
                LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(1),
                CurrentQualityControl.GetStatisticLimit(2),
                "+1s",
                0,
                LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetMeanValueOfService(),
                CurrentQualityControl.GetStatisticLimit(1),
                "x",
                0,
                LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(-1),
                CurrentQualityControl.GetMeanValueOfService(),
                "-1s",
                0,
                LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(-2),
                CurrentQualityControl.GetStatisticLimit(-1),
                "-2s",
                0,
                LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(-3),
                CurrentQualityControl.GetStatisticLimit(-2),
                "-3s",
                0,
                LabelMarkStyle.None));

            chartArea.AxisY.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(3),
                CurrentQualityControl.GetStatisticLimit(4),
                CurrentQualityControl.GetStatisticLimit(3).ToString(),
                0,
                LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(2),
                CurrentQualityControl.GetStatisticLimit(3),
                CurrentQualityControl.GetStatisticLimit(2).ToString(),
                0,
                LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(1),
                CurrentQualityControl.GetStatisticLimit(2),
                CurrentQualityControl.GetStatisticLimit(1).ToString(),
                0,
                LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetMeanValueOfService(),
                CurrentQualityControl.GetStatisticLimit(1),
                CurrentQualityControl.GetMeanValueOfService().ToString(),
                0,
                LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(-1),
                CurrentQualityControl.GetMeanValueOfService(),
                CurrentQualityControl.GetStatisticLimit(-1).ToString(),
                0,
                LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(-2),
                CurrentQualityControl.GetStatisticLimit(-1),
                CurrentQualityControl.GetStatisticLimit(-2).ToString(),
                0,
                LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(
                CurrentQualityControl.GetStatisticLimit(-3),
                CurrentQualityControl.GetStatisticLimit(-2),
                CurrentQualityControl.GetStatisticLimit(-3).ToString(),
                0,
                LabelMarkStyle.None));

            foreach (AppliedService appliedService in CurrentService.AppliedService)
            {
                _ = seriesLimit.Points.AddXY(
                    appliedService.FinishedDateTime.ToString("yyyy-MM-dd hh:mm:ss"),
                    appliedService.Result);
            }
            Chart.Series.Add(seriesLimit);
            WindowsFormsHost.Child = Chart;
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
                    _exportCommand = new RelayCommand(param =>
                    {
                        PresentationExporter exporter = new QualityControlPresentationExporter(
                            _qualityControl,
                            Chart,
                            CurrentExportType,
                            CurrentService);
                        exporter.Export();
                    });
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

        public WindowsFormsHost WindowsFormsHost
        {
            get
            {
                if (_windowsFormsHost == null)
                {
                    _windowsFormsHost = new WindowsFormsHost();
                }
                return _windowsFormsHost;
            }

            set
            {
                _windowsFormsHost = value;
                OnPropertyChanged();
            }
        }

        public double MeanDeviation
        {
            get => _meanDeviation; set
            {
                _meanDeviation = value;
                OnPropertyChanged();
            }
        }
        public double VariationCoefficient
        {
            get => _variationCoefficient; set
            {
                _variationCoefficient = value;
                OnPropertyChanged();
            }
        }

        public string CurrentRepresentationForm
        {
            get => _currentRepresentationForm; set
            {
                _currentRepresentationForm = value;
                switch (_currentRepresentationForm)
                {
                    case "графиком":
                        IsChartForm = true;
                        break;
                    case "таблицей":
                        IsChartForm = false;
                        break;
                    default:
                        break;

                }
                OnPropertyChanged();
            }
        }
        public ICollection<string> RepresentationForms
        {
            get
            {
                if (_representationForms == null)
                {
                    _representationForms = new List<string>
                    {
                        "графиком",
                        "таблицей"
                    };
                    CurrentRepresentationForm = _representationForms.First();
                }
                return _representationForms;
            }

            set
            {
                _representationForms = value;
                OnPropertyChanged();
            }
        }

        public bool IsChartForm
        {
            get => _isChartForm; set
            {
                _isChartForm = value;
                OnPropertyChanged();
            }
        }

        public Chart Chart
        {
            get => chart; set
            {
                chart = value;
                OnPropertyChanged();
            }
        }

        public QualityControlReport CurrentQualityControl
        {
            get => _qualityControl; set
            {
                _qualityControl = value;
                OnPropertyChanged();
            }
        }

        public string SelectedSavePath
        {
            get => _selectedSavePath; set
            {
                _selectedSavePath = value;
                OnPropertyChanged();
            }
        }
    }
}
