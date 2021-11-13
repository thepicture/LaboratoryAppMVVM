using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models;
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
        private RelayCommand _navigateToCreateOrEditOrderCommand;
        public TimeSpan CurrentTimeOfSession => _sessionTimer.TotalTimeLeft;

        public LaboratoryAssistantViewModel(ViewModelNavigationStore navigationStore, User user)
        {
            _navigationStore = navigationStore;
            User = user;
            Title = "Страница лаборанта";
            MessageBoxService = new MessageBoxService();
            _sessionTimer = new LaboratoryHaveTimeService(TimeSpan.FromMinutes(10), MessageBoxService, _navigationStore);
            _sessionTimer.TickChanged += OnTickChanged;
            _sessionTimer.Start();
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnTickChanged()
        {
            OnPropertyChanged(nameof(CurrentTimeOfSession));
        }

        private void OnCurrentViewModelChanged()
        {
            DisposerOnTypeEqual<LoginViewModel>.Dispose(_sessionTimer, _navigationStore);
        }

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

        public RelayCommand NavigateToCreateOrEditOrderCommand
        {
            get
            {
                if (_navigateToCreateOrEditOrderCommand == null)
                {
                    _navigateToCreateOrEditOrderCommand = new RelayCommand(param => _navigationStore.CurrentViewModel = new CreateOrEditOrderView(_navigationStore,
                        User,
                        param as Order,
                        MessageBoxService));
                }
                return _navigateToCreateOrEditOrderCommand;
            }
        }
    }
}
