using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private double _meanDeviation = 0;
        private double _variationCoefficient = 0;
        private string _currentRepresentationForm;
        private ICollection<string> _representationForms;
        private bool _isChartForm = true;
        private double _meanResultsValue;
        private double _positive1S;
        private double _positive2S;
        private double _positive3S;
        private double _negative1S;
        private double _negative2S;
        private double _negative3S;

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
                if (_currentService.AppliedService.Count != 0)
                {
                    MeanDeviation = GetMeanQuadrantDeviation();
                    VariationCoefficient = GetMeanQuadrantDeviation()
                                           / GetMeanValueOfService()
                                           * 100;
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
            _meanResultsValue = GetMeanValueOfService();
            _positive1S = _meanResultsValue + (GetMeanQuadrantDeviation() * 1);
            _positive2S = _meanResultsValue + (GetMeanQuadrantDeviation() * 2);
            _positive3S = _meanResultsValue + (GetMeanQuadrantDeviation() * 3);

            _negative1S = _meanResultsValue - (GetMeanQuadrantDeviation() * 1);
            _negative2S = _meanResultsValue - (GetMeanQuadrantDeviation() * 2);
            _negative3S = _meanResultsValue - (GetMeanQuadrantDeviation() * 3);

            Chart chart = new Chart();
            ChartArea chartArea = new ChartArea("ServiceArea");
            chartArea.AxisX.IsLabelAutoFit = false;
            chartArea.AxisX.LabelStyle.Angle = -45;
            chartArea.AxisX.Interval = 3;

            _ = new Axis(chartArea, AxisName.Y2);
            chart.ChartAreas.Add(chartArea);
            chart.Legends.Add(new Legend());
            Series seriesLimit = new Series(CurrentService.Name)
            {
                ChartType = SeriesChartType.Line,
                MarkerStyle = MarkerStyle.Cross,
                YAxisType = AxisType.Primary
            };

            chartArea.AxisY2.Enabled = AxisEnabled.True;

            chartArea.AxisY2.Minimum = Convert.ToDouble(_negative3S);
            chartArea.AxisY2.Maximum = Convert.ToDouble(_meanResultsValue + (GetMeanQuadrantDeviation() * 4));
            chartArea.AxisY2.Interval = GetMeanQuadrantDeviation();
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(_positive3S, _meanResultsValue + (GetMeanQuadrantDeviation() * 4), "+3s", 0, LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(_positive2S, _positive3S, "+2s", 0, LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(_positive1S, _positive2S, "+1s", 0, LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(GetMeanValueOfService(), _positive1S, "x", 0, LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(_negative1S, GetMeanValueOfService(), "-1s", 0, LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(_negative2S, _negative1S, "-2s", 0, LabelMarkStyle.None));
            chartArea.AxisY2.CustomLabels.Add(new CustomLabel(_negative3S, _negative2S, "-3s", 0, LabelMarkStyle.None));

            chartArea.AxisY.Minimum = Convert.ToDouble(_negative3S);
            chartArea.AxisY.Maximum = Convert.ToDouble(_meanResultsValue + (GetMeanQuadrantDeviation() * 4));
            chartArea.AxisY.Interval = GetMeanQuadrantDeviation();
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(_positive3S, _meanResultsValue + (GetMeanQuadrantDeviation() * 4), _positive3S.ToString(), 0, LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(_positive2S, _positive3S, _positive2S.ToString(), 0, LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(_positive1S, _positive2S, _positive1S.ToString(), 0, LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(GetMeanValueOfService(), _positive1S, GetMeanValueOfService().ToString(), 0, LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(_negative1S, GetMeanValueOfService(), _negative1S.ToString(), 0, LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(_negative2S, _negative1S, _negative2S.ToString(), 0, LabelMarkStyle.None));
            chartArea.AxisY.CustomLabels.Add(new CustomLabel(_negative3S, _negative2S, _negative3S.ToString(), 0, LabelMarkStyle.None));

            foreach (AppliedService appliedService in CurrentService.AppliedService)
            {
                _ = seriesLimit.Points.AddXY(appliedService.FinishedDateTime.ToString("yyyy-MM-dd hh:mm:ss"), appliedService.Result);
            }
            chart.Series.Add(seriesLimit);
            CurrentChart.Child = chart;
        }

        private double GetMeanQuadrantDeviation()
        {
            double meanValueOfService = GetMeanValueOfService();
            double meanQuadrantDeviation = 0;
            foreach (AppliedService appliedService in CurrentService.AppliedService)
            {
                meanQuadrantDeviation += Math.Pow(meanValueOfService - appliedService.Result, 2);
            }
            meanQuadrantDeviation /= CurrentService.AppliedService.Count;
            return meanQuadrantDeviation;
        }

        private double GetMeanValueOfService()
        {
            return CurrentService.AppliedService.Sum(s => s.Result) / CurrentService.AppliedService.Count;
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

        private void ExportChart()
        {

        }
    }
}
