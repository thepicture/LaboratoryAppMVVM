
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System.Collections.Generic;

namespace LaboratoryAppMVVM.ViewModels
{
    public class LaboratoryAssistantViewModel : ViewModelBase
    {
        private ViewModelNavigationStore _navigationStore;
        private List<AppliedService> _bioContent;
        private LaboratoryDatabaseEntities _context;

        public LaboratoryAssistantViewModel(ViewModelNavigationStore navigationStore, User user)
        {
            _navigationStore = navigationStore;
            User = user;
            Title = "Окно лаборанта";
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
    }
}
