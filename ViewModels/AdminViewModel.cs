using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore navigationStore;
        private List<HistoryOfLogin> _userLoginHistoriesList;
        private List<Service> _servicesList;
        private string _userLoginText = string.Empty;
        private LaboratoryDatabaseEntities _context;
        private List<string> _sortTypes;
        private string _currentSortType;

        public AdminViewModel(ViewModelNavigationStore navigationStore, User user)
        {
            this.navigationStore = navigationStore;
            Title = "Страница администратора";
            User = user;
        }

        public List<HistoryOfLogin> UserLoginHistoriesList
        {
            get
            {
                if (_userLoginHistoriesList == null)
                {
                    _userLoginHistoriesList = Context.HistoryOfLogin.ToList();
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
                    _servicesList = Context.Service.ToList();
                }
                return _servicesList;
            }

            set
            {
                _servicesList = value;
                OnPropertyChanged();
            }
        }

        public string UserLoginText
        {
            get => _userLoginText; set
            {
                _userLoginText = value;
                if (!string.IsNullOrWhiteSpace(_userLoginText))
                {
                    FilterUserLoginHistory();
                }
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

        public List<string> SortTypes
        {
            get
            {
                if (_sortTypes == null)
                {
                    _sortTypes = new List<string>
                    {
                        "Без сортировки",
                        "По возрастанию по дате попытки входа",
                        "По убыванию по дате попытки входа",
                    };
                    _currentSortType = _sortTypes.First();
                }
                return _sortTypes;
            }

            set
            {
                _sortTypes = value;
                OnPropertyChanged();
            }
        }
        public string CurrentSortType
        {
            get => _currentSortType; set
            {
                _currentSortType = value;
                FilterUserLoginHistory();
                OnPropertyChanged();
            }
        }

        private void FilterUserLoginHistory()
        {
            UserLoginHistoriesList = Context
                .HistoryOfLogin
                .Where(history => history.User.Login.ToLower().Contains(UserLoginText))
                .ToList();
            switch (SortTypes.IndexOf(CurrentSortType))
            {
                case 1:
                    UserLoginHistoriesList = UserLoginHistoriesList.OrderBy(history => history.DateTime).ToList();
                    break;
                case 2:
                    UserLoginHistoriesList = UserLoginHistoriesList.OrderByDescending(history => history.DateTime).ToList();
                    break;
                default:
                    break;
            }
        }
    }
}
