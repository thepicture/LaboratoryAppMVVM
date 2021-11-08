
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System.Collections.Generic;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LaboratoryResearcherViewModel : ViewModelBase
    {
        private ViewModelNavigationStore navigationStore;
        private List<Analyzer> _analyzersList;
        private LaboratoryDatabaseEntities _context;

        public LaboratoryResearcherViewModel(ViewModelNavigationStore navigationStore, Models.Entities.User user)
        {
            this.navigationStore = navigationStore;
            Title = "Страница лаборанта-исследователя";
            User = user;
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

        public User User { get; }
    }
}
