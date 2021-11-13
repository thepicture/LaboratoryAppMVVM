using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaboratoryAppMVVM.ViewModels
{
    public class CreateOrEditOrderView : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private Order _order;
        private List<AppliedService> _appliedServicesList;
        private LaboratoryDatabaseEntities _context;
        private RelayCommand _addServiceToServicesViewCommand;
        private RelayCommand _navigateToLaboratoryAssistantViewModel;

        public CreateOrEditOrderView(ViewModelNavigationStore navigationStore, User user, Order order)
        {
            _navigationStore = navigationStore;
            User = user;
            Order = order;
        }

        public Order Order
        {
            get
            {
                if (_order == null)
                {
                    _order = new Order();
                }
                return _order;
            }

            set
            {
                _order = value;
                OnPropertyChanged();
            }
        }

        public List<AppliedService> AppliedServicesList
        {
            get
            {
                if (_appliedServicesList == null)
                {
                    _appliedServicesList = Order.AppliedService.ToList();
                }
                return _appliedServicesList;
            }

            set
            {
                _appliedServicesList = value;
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
        }

        public RelayCommand AddServiceToServicesViewCommand
        {
            get
            {
                if (_addServiceToServicesViewCommand == null)
                {
                    _addServiceToServicesViewCommand = new RelayCommand(param => AppliedServicesList.Add(new AppliedService()));
                }
                return _addServiceToServicesViewCommand;
            }
        }

        public RelayCommand NavigateToLaboratoryAssistantViewModel
        {
            get
            {
                if (_navigateToLaboratoryAssistantViewModel == null)
                {
                    _navigateToLaboratoryAssistantViewModel =
                        new RelayCommand(param => _navigationStore.CurrentViewModel =
                        new LaboratoryAssistantViewModel(_navigationStore, User));
                }
                return _navigateToLaboratoryAssistantViewModel;
            }
        }
    }
}
