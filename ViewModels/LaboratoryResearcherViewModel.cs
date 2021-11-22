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
    public class LaboratoryResearcherViewModel : ViewModelBase
    {
        private readonly IWindowService _laboratoryWindowService;
        private List<Analyzer> _analyzers;
        private LaboratoryDatabaseEntities _context;
        private readonly LaboratoryHaveTimeService _sessionTimer;
        public string CurrentTimeOfSession
        {
            get
            {
                return _sessionTimer.TotalTimeLeft.ToString("hh\\:mm");
            }
        }

        private ICommand _openAnalyzerViewModelCommand;
        public event Action SessionEnd;

        public LaboratoryResearcherViewModel(ViewModelNavigationStore navigationStore,
                                             User user,
                                             IWindowService laboratoryWindowService)
        {
            NavigationStore = navigationStore;
            _laboratoryWindowService = laboratoryWindowService;
            Title = "Страница лаборанта-исследователя";
            User = user;
            MessageBoxService = new MessageBoxService();
            _sessionTimer = new LaboratoryHaveTimeService(TimeSpan.FromMinutes(10),
                                                          MessageBoxService,
                                                          NavigationStore);
            _sessionTimer.TickChanged += OnTickChanged;
            _sessionTimer.Start();
            NavigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
        }

        private void OnTickChanged()
        {
            OnPropertyChanged(nameof(CurrentTimeOfSession));
        }

        private void OnCurrentViewModelChanged()
        {
            DisposerOnTypeEqual<LoginViewModel>.Dispose(_sessionTimer, NavigationStore);
            SessionEnd?.Invoke();
        }

        public List<Analyzer> Analyzers
        {
            get
            {
                if (_analyzers == null)
                {
                    _analyzers = new List<Analyzer>(Context.Analyzer);
                }
                return _analyzers;
            }

            set
            {
                _analyzers = value;
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

        public ICommand OpenAnalyzerViewModelCommand
        {
            get
            {
                if (_openAnalyzerViewModelCommand == null)
                {
                    _openAnalyzerViewModelCommand = new RelayCommand(param =>
                    {
                        _laboratoryWindowService
                        .ShowWindow(new AnalyzerViewModel(param as Analyzer,
                                                          this));
                    });
                }
                return _openAnalyzerViewModelCommand;
            }
        }

        public ViewModelNavigationStore NavigationStore { get; }
    }
}
