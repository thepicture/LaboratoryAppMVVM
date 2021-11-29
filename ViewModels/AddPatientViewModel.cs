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
        private List<TypeOfInsurancePolicy> _policyTypes;
        private List<InsuranceCompany> _insuranceCompanies;
        private LaboratoryDatabaseEntities _context;
        private InsuranceCompany _selectedInsuranceCompany;
        private TypeOfInsurancePolicy _selectedPolicyType;

        public AddPatientViewModel(
            ViewModelNavigationStore navigationStore,
            IMessageService messageBoxService,
            CreateOrEditOrderViewModel createOrEditOrderViewModel,
            Patient patient = null)
        {
            _createOrEditOrderViewModel = createOrEditOrderViewModel;
            if (patient != null)
            {
                CurrentPatient = patient;
                SelectedInsuranceCompany = InsuranceCompanies
                    .First(c => c.Id == patient.InsuranceCompanyId);
                SelectedPolicyType = PolicyTypes
                    .First(t => t.Id == patient.TypeOfInsurancePolicyId);
            }
            _navigationStore = navigationStore;
            MessageService = messageBoxService;
            Title = "Добавление нового пациента";
        }

        public Patient CurrentPatient
        {
            get
            {
                if (_currentPatient == null)
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
                        {
                            _createOrEditOrderViewModel.Context.ChangeTracker.Entries()
                            .ToList()
                            .ForEach(e => e.Reload());
                            _navigationStore.CurrentViewModel =
                            _createOrEditOrderViewModel;
                        });
                }
                return _returnToEditOrderViewModelCommand;
            }
        }
        public List<TypeOfInsurancePolicy> PolicyTypes
        {
            get
            {
                if (_policyTypes == null)
                {
                    _policyTypes = Context.TypeOfInsurancePolicy.ToList();
                }
                return _policyTypes;
            }
        }
        public List<InsuranceCompany> InsuranceCompanies
        {
            get
            {
                if (_insuranceCompanies == null)
                {
                    _insuranceCompanies = Context.InsuranceCompany.ToList();
                }
                return _insuranceCompanies;
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
            InsertValuesIntoPatientEntityAndAddPatientIfNew();
            TryToSavePatient();
        }

        private void InsertValuesIntoPatientEntityAndAddPatientIfNew()
        {
            CurrentPatient.InsuranceCompany = SelectedInsuranceCompany;
            CurrentPatient.TypeOfInsurancePolicy = SelectedPolicyType;
            if (CurrentPatient.Id == 0)
            {
                _ = Context.Patient.Add(CurrentPatient);
            }
        }

        private void TryToSavePatient()
        {
            try
            {
                _ = Context.SaveChanges();
                MessageService.ShowInformation("Пациент " +
                    "успешно сохранён!");
            }
            catch (Exception ex)
            {
                MessageService.ShowError("Не удалось сохранить данные" +
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
                    _context = _createOrEditOrderViewModel.Context;
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
