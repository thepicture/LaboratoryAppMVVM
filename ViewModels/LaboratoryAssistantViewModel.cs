using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LaboratoryAssistantViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private List<AppliedService> _appliedServices;
        private LaboratoryDatabaseEntities _context;
        private readonly HaveTimeServiceBase _sessionTimer;
        private System.Windows.Input.ICommand _navigateToCreateOrEditOrderCommand;
        public TimeSpan CurrentTimeOfSession => _sessionTimer.TotalTimeLeft;

        public LaboratoryAssistantViewModel(ViewModelNavigationStore navigationStore,
                                            User user)
        {
            _navigationStore = navigationStore;
            User = user;
            Title = "Страница лаборанта";
            MessageService = new MessageBoxService();
            _sessionTimer = new LaboratoryHaveTimeService(TimeSpan.FromMinutes(10),
                                                          MessageService,
                                                          _navigationStore);
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

        public List<AppliedService> AppliedServices
        {
            get
            {
                if (_appliedServices == null)
                {
                    _appliedServices = new List<AppliedService>(Context.AppliedService);
                }
                return _appliedServices;
            }

            set
            {
                _appliedServices = value;
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

        public System.Windows.Input.ICommand NavigateToCreateOrEditOrderCommand
        {
            get
            {
                if (_navigateToCreateOrEditOrderCommand == null)
                {
                    _navigateToCreateOrEditOrderCommand = new Commands.RelayCommand(param =>
                    {
                        _navigationStore.CurrentViewModel =
                        new CreateOrEditOrderViewModel(_navigationStore,
                                                       User,
                                                       param as Order,
                                                       MessageService,
                                                       this);
                    });
                }
                return _navigateToCreateOrEditOrderCommand;
            }
        }
    }
}
