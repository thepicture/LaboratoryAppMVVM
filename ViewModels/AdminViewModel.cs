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
        private List<HistoryOfLogin> _userLoginHistories;
        private List<Service> _services;
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

        public List<HistoryOfLogin> UserLoginHistories
        {
            get
            {
                if (_userLoginHistories == null)
                {
                    _userLoginHistories = Context.HistoryOfLogin.ToList();
                }
                return _userLoginHistories;
            }

            set
            {
                _userLoginHistories = value;
                OnPropertyChanged();
            }
        }
        public List<Service> Services
        {
            get
            {
                if (_services == null)
                {
                    _services = Context.Service.ToList();
                }
                return _services;
            }

            set
            {
                _services = value;
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
            UserLoginHistories = Context
                .HistoryOfLogin
                .Where(history => history.User.Login.ToLower().Contains(UserLoginText))
                .ToList();
            OrderLoginHistoriesByCurrentSortType();
        }

        private void OrderLoginHistoriesByCurrentSortType()
        {
            switch (SortTypes.IndexOf(CurrentSortType))
            {
                case 1:
                    UserLoginHistories = UserLoginHistories.OrderBy(history => history.DateTime).ToList();
                    break;
                case 2:
                    UserLoginHistories = UserLoginHistories.OrderByDescending(history => history.DateTime).ToList();
                    break;
                default:
                    break;
            }
        }
    }
}
