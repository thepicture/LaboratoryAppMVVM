using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AccountantViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _viewModelNavigationStore;
        private List<AppliedService> _appliedServicesList;

        public AccountantViewModel(ViewModelNavigationStore viewModelNavigationStore, User user)
        {
            _viewModelNavigationStore = viewModelNavigationStore;
            User = user;
        }

        public List<AppliedService> AppliedServicesList
        {
            get
            {
                if (_appliedServicesList == null)
                {
                    _appliedServicesList = new LaboratoryDatabaseEntities().AppliedService.ToList();
                }
                return _appliedServicesList;
            }

            set
            {
                _appliedServicesList = value;
                OnPropertyChanged();
            }
        }
    }
}
