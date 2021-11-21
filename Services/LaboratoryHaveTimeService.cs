using LaboratoryAppMVVM.Models.Exceptions;
using LaboratoryAppMVVM.Stores;
using LaboratoryAppMVVM.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace LaboratoryAppMVVM.Services
{
    public class LaboratoryHaveTimeService : IHaveTimeService
    {
        public event Action TickChanged;
        public LaboratoryHaveTimeService(TimeSpan sessionTimeSpan)
        {
            _timer = new DispatcherTimer(priority: DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(0.01),
            };
            TotalTimeLeft = sessionTimeSpan;
            _timer.Tick += OnSessionTimerTick;
        }

        public LaboratoryHaveTimeService(TimeSpan sessionTimeSpan,
                                         IMessageService messageBoxService,
                                         ViewModelNavigationStore navigationStore) : this(sessionTimeSpan)
        {
            MessageBoxService = messageBoxService;
            NavigationStore = navigationStore;
        }

        private void OnSessionTimerTick(object sender, EventArgs e)
        {
            if (TotalTimeLeft == new TimeSpan(0, 5, 0))
            {
                _ = Task.Run(ShowSessionExitSoonMessage);
            }
            if (TotalTimeLeft == TimeSpan.Zero)
            {
                _ = Task.Run(ShowSessionIsDoneMessage);
                Stop();
                NavigationStore.CurrentViewModel = new LoginViewModel(NavigationStore, MessageBoxService, new LaboratoryLoginService());
            }
            TotalTimeLeft -= TimeSpan.FromSeconds(1);
        }

        private void ShowSessionExitSoonMessage()
        {
            MessageBoxService.ShowInformation($"Через {Convert.ToInt32(Math.Round(TotalTimeLeft.TotalMinutes))} минут " +
                "сессия завершится");
        }

        private void ShowSessionIsDoneMessage()
        {
            MessageBoxService.ShowInformation("Необходимо выполнить " +
                "кварцевание помещений. " +
                $"Сессия завершена. " +
                $"Вы сможете зайти через " +
                $"{TimeSpan.FromMinutes(30).TotalMinutes} " +
                $"минут.");
        }


        private void OnTickChanged()
        {
            TickChanged?.Invoke();
        }

        public TimeSpan TotalTimeLeft
        {
            get => _totalTimeLeft; set
            {
                _totalTimeLeft = value;
                OnTickChanged();
            }
        }

        public IMessageService MessageBoxService { get; }
        public ViewModelNavigationStore NavigationStore { get; }

        private readonly DispatcherTimer _timer;
        private TimeSpan _totalTimeLeft;

        public void Start()
        {
            if (_timer.IsEnabled)
            {
                throw new SessionIsAlreadyEnabledException("Attempt to invoke the enabled timer");
            }
            _timer.Start();
        }

        public void Stop()
        {
            if (!_timer.IsEnabled)
            {
                return;
            }
            _timer.Stop();
        }
    }
}