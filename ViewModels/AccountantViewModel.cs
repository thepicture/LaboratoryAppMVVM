using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AccountantViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _viewModelNavigationStore;
        private List<AppliedService> _appliedServices;

        public AccountantViewModel(ViewModelNavigationStore viewModelNavigationStore, User user)
        {
            _viewModelNavigationStore = viewModelNavigationStore;
            Title = "Страница бухгалтера";
            User = user;
        }

        public List<AppliedService> AppliedServices
        {
            get
            {
                if (_appliedServices == null)
                {
                    _appliedServices = new LaboratoryDatabaseEntities().AppliedService.ToList();
                }
                return _appliedServices;
            }

            set
            {
                _appliedServices = value;
                OnPropertyChanged();
            }
        }
    }
}
