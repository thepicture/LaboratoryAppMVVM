using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LaboratoryAppMVVM.ViewModels
{
    public class CreateOrEditOrderView : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private Order _order;
        private ObservableCollection<AppliedService> _appliedServicesList;
        private LaboratoryDatabaseEntities _context;
        private RelayCommand _addServiceToServicesViewCommand;
        private RelayCommand _navigateToLaboratoryAssistantViewModel;
        private RelayCommand _enterTooltipCommand;
        private string _tubeIdTooltipText = "Введите код пробирки...";
        private string _tubeId;

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

        public ObservableCollection<AppliedService> AppliedServicesList
        {
            get
            {
                if (_appliedServicesList == null)
                {
                    _appliedServicesList = new ObservableCollection<AppliedService>(Order.AppliedService);
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
                    _addServiceToServicesViewCommand =
                        new RelayCommand(param => AppliedServicesList.Add(new AppliedService()));
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

        public string TubeIdTooltipText
        {
            get
            {
                if (Context.Order.Any())
                {
                    _tubeIdTooltipText = Convert.ToString(Context.Order.ToList().Last().Id + 1);
                }
                return _tubeIdTooltipText;
            }

            set
            {
                _tubeIdTooltipText = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand EnterTooltipCommand
        {
            get
            {
                if (_enterTooltipCommand == null)
                {
                    _enterTooltipCommand = new RelayCommand(param =>
                    {
                        if (string.IsNullOrWhiteSpace(TubeId))
                        {
                            TubeId = TubeIdTooltipText;
                        }
                    });
                }
                return _enterTooltipCommand;
            }
        }

        public string TubeId
        {
            get => _tubeId; set
            {
                _tubeId = value;
                OnPropertyChanged();
            }
        }
    }
}
