
using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LaboratoryAssistantViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private List<AppliedService> _bioContent;
        private LaboratoryDatabaseEntities _context;
        private readonly LaboratoryHaveTimeService _sessionTimer;
        private RelayCommand _navigateToLoginCommand;
        public TimeSpan CurrentTimeOfSession => _sessionTimer.TotalTimeLeft;

        public LaboratoryAssistantViewModel(ViewModelNavigationStore navigationStore, User user)
        {
            _navigationStore = navigationStore;
            User = user;
            Title = "Страница лаборанта";
            MessageBoxService = new MessageBoxService();
            _sessionTimer = new LaboratoryHaveTimeService(TimeSpan.FromMinutes(1));
            _sessionTimer.TickChanged += OnSessionTimerTickChanged;
            _sessionTimer.Start();
        }

        private void OnSessionTimerTickChanged()
        {
            OnPropertyChanged(nameof(CurrentTimeOfSession));
            if (CurrentTimeOfSession == new TimeSpan(0, 0, 30))
            {
                Task.Run(ShowSessionExitSoonMessage);
            }
            if (CurrentTimeOfSession == TimeSpan.Zero)
            {
                Task.Run(ShowSessionIsDoneMessage);
                _sessionTimer.Stop();
                NavigateToLoginCommand.Execute();
            }
        }

        private void ShowSessionExitSoonMessage()
        {
            MessageBoxService.ShowInformation($"Через {CurrentTimeOfSession.TotalMinutes} минут " +
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

        public User User { get; }
        public List<AppliedService> BioContent
        {
            get
            {
                if (_bioContent == null)
                {
                    _bioContent = new List<AppliedService>(Context.AppliedService);
                }
                return _bioContent;
            }

            set
            {
                _bioContent = value;
                OnPropertyChanged();
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

            set
            {
                _context = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateToLoginCommand
        {
            get
            {
                if (_navigateToLoginCommand == null)
                {
                    _navigateToLoginCommand = new RelayCommand(param => _navigationStore.CurrentViewModel = new LoginViewModel(_navigationStore,
                                                                                                                               MessageBoxService as MessageBoxService,
                                                                                                                               new LaboratoryLoginService()));
                }
                return _navigateToLoginCommand;
            }
        }
    }
}
