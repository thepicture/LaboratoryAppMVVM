using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Http;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AnalyzerViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _viewModelNavigationStore;
        private readonly LaboratoryDatabaseEntities _context;
        private Analyzer _analyzer;
        private ObservableCollection<AppliedService> _notAcceptedServicesList;
        private bool _isNotOnLoginPage = false;
        private RelayCommand _navigateToLoginPageCommand;
        private RelayCommand _sendServiceToResearchingCommand;
        private ObservableCollection<AppliedService> _sentServicesList;

        public AnalyzerViewModel(ViewModelNavigationStore viewModelNavigationStore,
                                 Analyzer analyzer,
                                 IMessageBoxService messageBoxService,
                                 LaboratoryDatabaseEntities context)
        {
            _viewModelNavigationStore = viewModelNavigationStore;
            Analyzer = analyzer;
            _context = context;
            MessageBoxService = messageBoxService;
            DispatcherTimer dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            dispatcherTimer.Tick += OnUpdateServicesValuesTick;
            dispatcherTimer.Start();
        }

        private void OnUpdateServicesValuesTick(object sender, EventArgs e)
        {
            UpdateServicesList();
        }

        private void UpdateServicesList()
        {
            _ = Task.Run(() =>
            {
                NotAcceptedServicesList = new ObservableCollection<AppliedService>(
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

        public ObservableCollection<AppliedService> NotAcceptedServicesList
        {
            get
            {
                if (_notAcceptedServicesList == null)
                {
                    _notAcceptedServicesList = new ObservableCollection<AppliedService>
                        (
                            Analyzer.AppliedService.Where(s => !s.IsAccepted)
                        );
                }
                return _notAcceptedServicesList;
            }

            set
            {
                _notAcceptedServicesList = value;
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
                                           new MessageBoxService(),
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
                        SendService(param as AppliedService);
                    });
                }
                return _sendServiceToResearchingCommand;
            }
        }

        public ObservableCollection<AppliedService> SentServicesList
        {
            get => _sentServicesList; set
            {
                _sentServicesList = value;
                OnPropertyChanged();
            }
        }

        private void SendService(AppliedService appliedService)
        {
            try
            {

                WebClient client = new ThirtySecondsTimeoutWebClient();
                client.Headers.Add("Content-Type", "application/json");
                string webApiURL = $"http://localhost:60954/api/analyzer/"
                    + Analyzer.Name;
                string jsonData = "{\"patient\":"
                                  + appliedService.PatientId
                                  + ",\"services\":[{\"serviceCode\": "
                                  + appliedService.Id
                                  + "}]}";
                _ = client.UploadData(webApiURL,
                                  "POST",
                                  Encoding.UTF8.GetBytes(jsonData));
                UpdateServicesList();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Success)
                {
                    MessageBoxService.ShowError("Произошла ошибка, " +
                        "но запрос обработан. " +
                        "Ошибка: " + ex.Message);
                }
                else
                {
                    MessageBoxService.ShowError("Произошла ошибка " +
                        "при отправке услуги. Вероятно, прошло 30 секунд " +
                        "с момента попытки отправки услуги " +
                        "на исследование. " +
                        "Пожалуйста, попробуйте ещё раз. " +
                        "Ошибка: " + ex.Message);
                }
            }
        }
    }
}