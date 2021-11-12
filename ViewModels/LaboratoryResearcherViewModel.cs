using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LaboratoryResearcherViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private List<Analyzer> _analyzersList;
        private LaboratoryDatabaseEntities _context;
        private readonly LaboratoryHaveTimeService _sessionTimer;
        public TimeSpan CurrentTimeOfSession => _sessionTimer.TotalTimeLeft;

        public LaboratoryResearcherViewModel(ViewModelNavigationStore navigationStore, User user)
        {
            _navigationStore = navigationStore;
            Title = "Страница лаборанта-исследователя";
            User = user;
            MessageBoxService = new MessageBoxService();
            _sessionTimer = new LaboratoryHaveTimeService(TimeSpan.FromMinutes(1), MessageBoxService, _navigationStore);
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

        public List<Analyzer> AnalyzersList
        {
            get
            {
                if (_analyzersList == null)
                {
                    _analyzersList = new List<Analyzer>(Context.Analyzer);
                }
                return _analyzersList;
            }

            set
            {
                _analyzersList = value;
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
    }
}
