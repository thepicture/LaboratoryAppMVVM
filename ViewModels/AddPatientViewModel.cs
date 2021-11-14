using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AddPatientViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly CreateOrEditOrderViewModel _createOrEditOrderViewModel;
        private Patient _currentPatient;
        private RelayCommand _savePatientCommand;
        private RelayCommand _returnToEditOrderViewModelCommand;
        private List<TypeOfInsurancePolicy> _policyTypesList;
        private List<InsuranceCompany> _insuranceCompaniesList;
        private LaboratoryDatabaseEntities _context;
        private InsuranceCompany _selectedInsuranceCompany;
        private TypeOfInsurancePolicy _selectedPolicyType;

        public AddPatientViewModel(ViewModelNavigationStore navigationStore,
                                   IMessageBoxService messageBoxService,
                                   CreateOrEditOrderViewModel createOrEditOrderViewModel)
        {
            _navigationStore = navigationStore;
            MessageBoxService = messageBoxService;
            _createOrEditOrderViewModel = createOrEditOrderViewModel;
            Title = "Добавление нового пациента";
        }

        public Patient CurrentPatient
        {
            get
            {
                if(_currentPatient == null)
                {
                    _currentPatient = new Patient();
                }
                return _currentPatient;
            }

            set
            {
                _currentPatient = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ReturnToEditOrderViewModelCommand
        {
            get
            {
                if (_returnToEditOrderViewModelCommand == null)
                {
                    _returnToEditOrderViewModelCommand =
                        new RelayCommand(param =>
                        _navigationStore.CurrentViewModel = _createOrEditOrderViewModel
                    );
                }
                return _returnToEditOrderViewModelCommand;
            }
        }
        public List<TypeOfInsurancePolicy> PolicyTypesList
        {
            get
            {
                if (_policyTypesList == null)
                {
                    _policyTypesList = Context.TypeOfInsurancePolicy.ToList();
                }
                return _policyTypesList;
            }
        }
        public List<InsuranceCompany> InsuranceCompaniesList
        {
            get
            {
                if (_insuranceCompaniesList == null)
                {
                    _insuranceCompaniesList = Context.InsuranceCompany.ToList();
                }
                return _insuranceCompaniesList;
            }
        }
        public RelayCommand SavePatientCommand
        {
            get
            {
                if (_savePatientCommand == null)
                {
                    _savePatientCommand = new RelayCommand(param => SavePatient());
                }
                return _savePatientCommand;
            }
        }

        private void SavePatient()
        {
            CurrentPatient.InsuranceCompany = SelectedInsuranceCompany;
            CurrentPatient.TypeOfInsurancePolicy = SelectedPolicyType;
            Context.Patient.Add(CurrentPatient);
            try
            {
                Context.SaveChanges();
                MessageBoxService.ShowInformation("Пациент " +
                    "успешно сохранён!");
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError("Не удалось сохранить " +
                    "пациента. Пожалуйста, попробуйте ещё раз. " +
                    "Ошибка: " + ex.Message);
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

        public TypeOfInsurancePolicy SelectedPolicyType
        {
            get => _selectedPolicyType; set
            {
                _selectedPolicyType = value;
                OnPropertyChanged();
            }
        }

        public InsuranceCompany SelectedInsuranceCompany
        {
            get => _selectedInsuranceCompany; set
            {
                _selectedInsuranceCompany = value;
                OnPropertyChanged();
            }
        }
    }
}
