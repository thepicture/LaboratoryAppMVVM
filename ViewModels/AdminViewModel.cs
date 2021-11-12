using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private ViewModelNavigationStore navigationStore;
        private List<HistoryOfLogin> _userLoginHistoriesList;
        private List<Service> _servicesList;

        public AdminViewModel(ViewModelNavigationStore navigationStore, User user)
        {
            this.navigationStore = navigationStore;
            Title = "Страница администратора";
        }

        public List<HistoryOfLogin> UserLoginHistoriesList
        {
            get
            {
                if (_userLoginHistoriesList == null)
                {
                    _userLoginHistoriesList = new LaboratoryDatabaseEntities().HistoryOfLogin.ToList();
                }
                return _userLoginHistoriesList;
            }

            set
            {
                _userLoginHistoriesList = value;
                OnPropertyChanged();
            }
        }
        public List<Service> ServicesList
        {
            get
            {
                if (_servicesList == null)
                {
                    _servicesList = new LaboratoryDatabaseEntities().Service.ToList();
                }
                return _servicesList;
            }

            set
            {
                _servicesList = value;
                OnPropertyChanged();
            }
        }
    }
}
