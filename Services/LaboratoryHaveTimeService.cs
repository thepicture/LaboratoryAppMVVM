using LaboratoryAppMVVM.Models.Exceptions;
using System;
using System.Windows;
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
                Interval = TimeSpan.FromSeconds(.1),
            };
            TotalTimeLeft = sessionTimeSpan;
            _timer.Tick += OnSessionTimerTick;
        }

        private void OnSessionTimerTick(object sender, EventArgs e)
        {
            TotalTimeLeft -= TimeSpan.FromSeconds(1);
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