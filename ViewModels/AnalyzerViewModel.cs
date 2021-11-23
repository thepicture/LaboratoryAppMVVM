using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.HttpClasses;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AnalyzerViewModel : ViewModelBase
    {
        private const int timerTimeout = 5;
        private readonly ViewModelNavigationStore _viewModelNavigationStore;
        private readonly LaboratoryDatabaseEntities _context;
        private Analyzer _analyzer;
        private ObservableCollection<AppliedService> _notAcceptedServices;
        private bool _isNotOnLoginPage = false;
        private RelayCommand _navigateToLoginPageCommand;
        private RelayCommand _sendServiceToResearchingCommand;
        private ObservableCollection<AppliedService> _sentServices;
        private ResearchStatus _status;
        private bool _isWaitingForResearchCompletion = false;

        public AnalyzerViewModel(Analyzer analyzer,
                                 LaboratoryResearcherViewModel parentViewModel)
        {
            _viewModelNavigationStore = parentViewModel.NavigationStore;
            Analyzer = analyzer;
            _context = parentViewModel.Context;
            MessageService = parentViewModel.MessageService;
            DispatcherTimer dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(timerTimeout)
            };
            dispatcherTimer.Tick += OnUpdateServiceValue;
            dispatcherTimer.Start();
            parentViewModel.SessionEnd += OnSessionEnd;
        }

        private void OnSessionEnd()
        {
            foreach (System.Windows.Window window in App.Current.Windows)
            {
                if (window.Content == this)
                {
                    window.Close();
                }
            }
        }

        private void OnUpdateServiceValue(object sender, EventArgs e)
        {
            UpdateServices();
            GetResearchStatusInPercent();
        }

        private void UpdateServices()
        {
            _ = Task.Run(() =>
            {
                NotAcceptedServices = new ObservableCollection<AppliedService>(
                new LaboratoryDatabaseEntities().Analyzer
                      .Find(Analyzer.Id)
                      .AppliedService
                      .Where(s => !s.IsAccepted)
                      .ToList());
            });
        }

        public Analyzer Analyzer
        {
            get => _analyzer; set
            {
                _analyzer = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AppliedService> NotAcceptedServices
        {
            get
            {
                if (_notAcceptedServices == null)
                {
                    _notAcceptedServices = new ObservableCollection<AppliedService>
                        (
                            Analyzer.AppliedService.Where(s => !s.IsAccepted)
                        );
                }
                return _notAcceptedServices;
            }

            set
            {
                _notAcceptedServices = value;
                OnPropertyChanged();
            }
        }

        public bool IsNotOnLoginPage
        {
            get => _isNotOnLoginPage; set
            {
                _isNotOnLoginPage = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateToLoginPageCommand
        {
            get
            {
                if (_navigateToLoginPageCommand == null)
                {
                    _navigateToLoginPageCommand = new RelayCommand(param =>
                    {
                        _viewModelNavigationStore.CurrentViewModel =
                        new LoginViewModel(_viewModelNavigationStore,
                                           MessageService,
                                           new LaboratoryLoginService());
                    });
                }
                return _navigateToLoginPageCommand;
            }
        }

        public RelayCommand SendServiceToResearchingCommand
        {
            get
            {
                if (_sendServiceToResearchingCommand == null)
                {
                    _sendServiceToResearchingCommand = new RelayCommand(param =>
                    {
                        RunNewServiceResearchTask(param as AppliedService);
                    });
                }
                return _sendServiceToResearchingCommand;
            }
        }

        public ObservableCollection<AppliedService> SentServices
        {
            get
            {
                if (_sentServices == null)
                {
                    _sentServices = new ObservableCollection<AppliedService>();
                }
                return _sentServices;
            }

            set
            {
                _sentServices = value;
                OnPropertyChanged();
            }
        }

        public ResearchStatus Status
        {
            get => _status; set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        public bool IsWaitingForResearchCompletion
        {
            get => _isWaitingForResearchCompletion; set
            {
                _isWaitingForResearchCompletion = value;
                OnPropertyChanged();
            }
        }

        private void GetResearchStatusInPercent()
        {
            if (IsWaitingForResearchCompletion)
            {
                SendGetRequest();
            }
        }

        private void SendGetRequest()
        {
            string url = $"http://localhost:60954/api/analyzer/"
                           + Analyzer.Name;
            IGettable statusGetter = new JsonResearchStatusGetter(url);
            byte[] response = statusGetter.Get();
            DataContractJsonSerializer contractJsonSerializer =
                new DataContractJsonSerializer(typeof(ResearchStatus));
            Status = (ResearchStatus)contractJsonSerializer
                .ReadObject(new MemoryStream(response));
            if (Status.Progress == ProgressState.Fulfilled)
            {
                DisableWaitMode();
            }
        }

        private void DisableWaitMode()
        {
            IsWaitingForResearchCompletion = false;
            SentServices.Clear();
        }

        private async void RunNewServiceResearchTask(AppliedService appliedService)
        {
            await Task.Run(() =>
            {
                PostServiceResearchRequest(appliedService);
            });
        }

        private void PostServiceResearchRequest(AppliedService appliedService)
        {
            try
            {
                PostCurrentAppliedService(appliedService);
                UpdateServices();
            }
            catch (WebException ex)
            {
                ShowException(ex);
            }
        }

        private void ShowException(WebException ex)
        {
            if (ex.Status == WebExceptionStatus.Success)
            {
                MessageService.ShowError("Произошла ошибка, " +
                    "но запрос обработан. " +
                    "Ошибка: " + ex.Message);
            }
            else
            {
                MessageService.ShowError("Произошла ошибка " +
                    "при отправке услуги. Вероятно, прошло 30 секунд " +
                    "с момента попытки отправки услуги " +
                    "на исследование. " +
                    "Пожалуйста, попробуйте ещё раз. " +
                    "Ошибка: " + ex.Message);
            }
        }

        private void PostCurrentAppliedService(AppliedService appliedService)
        {
            string url = $"http://localhost:60954/api/analyzer/"
                            + Analyzer.Name;
            string jsonData = "{\"patient\":"
                                          + appliedService.PatientId
                                          + ",\"services\":[{\"serviceCode\": "
                                          + appliedService.Id
                                          + "}]}";
            IPostable jsonServicePoster = new JsonServicePoster(url, jsonData);
            _ = jsonServicePoster.Post();
            SentServices.Add(appliedService);
            if (!IsWaitingForResearchCompletion)
            {
                IsWaitingForResearchCompletion = true;
            }
        }
    }
}