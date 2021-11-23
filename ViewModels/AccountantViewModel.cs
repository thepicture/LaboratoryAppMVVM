using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exports;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AccountantViewModel : ViewModelBase
    {
        private ICollection<AppliedService> _appliedServices;
        private ICommand _createInsuranceCompaniesReportCommand;
        private DateTime _fromPeriod = DateTime.Now;
        private DateTime _toPeriod = DateTime.Now;
        private string _dateValidationErrors = "";
        private LaboratoryDatabaseEntities _context;
        private bool _isCorrectPeriod = false;
        private FolderBrowserDialog _folderBrowserDialog;
        private List<InsuranceCompany> _reportInsuranceCompanies;

        public AccountantViewModel(User user)
        {
            Title = "Страница бухгалтера";
            User = user;
        }

        public ICollection<AppliedService> AppliedServices
        {
            get
            {
                if (_appliedServices == null)
                {
                    _appliedServices = new LaboratoryDatabaseEntities()
                        .AppliedService
                        .ToList();
                }
                return _appliedServices;
            }

            set
            {
                _appliedServices = value;
                OnPropertyChanged();
            }
        }

        public ICommand CreateInsuranceCompaniesReportCommand
        {
            get
            {
                if (_createInsuranceCompaniesReportCommand == null)
                {
                    _createInsuranceCompaniesReportCommand = new RelayCommand(param =>
                    {
                        CreateInsuranceCompanyReport();
                    });
                }
                return _createInsuranceCompaniesReportCommand;
            }
        }

        public DateTime FromPeriod
        {
            get => _fromPeriod; set
            {
                _fromPeriod = value;
                CheckIfPeriodIsCorrect();
                OnPropertyChanged();
            }
        }

        private void CheckIfPeriodIsCorrect()
        {
            if (AreNotCorrectPeriodValues())
            {
                DateValidationErrors = "Укажите корректный период выше";
            }
            else
            {
                DateValidationErrors = "";
            }
        }

        private bool AreNotCorrectPeriodValues()
        {
            return FromPeriod != null
                && ToPeriod != null
                && FromPeriod <= ToPeriod;
        }

        public DateTime ToPeriod
        {
            get => _toPeriod; set
            {
                _toPeriod = value;
                CheckIfPeriodIsCorrect();
                OnPropertyChanged();
            }
        }
        public string DateValidationErrors
        {
            get => _dateValidationErrors; set
            {
                _dateValidationErrors = value;
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

        public bool IsCorrectPeriod
        {
            get => _isCorrectPeriod; set
            {
                _isCorrectPeriod = value;
                OnPropertyChanged();
            }
        }

        private void CreateInsuranceCompanyReport()
        {
            PrepareInsuranceCompaniesInPeriod();
            if (_reportInsuranceCompanies.Count == 0)
            {
                DateValidationErrors = "За указанный период компаний не найдено";
                return;
            }
            if (_folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                ExportInsuranceCompanyReport();
            }
        }

        private void PrepareInsuranceCompaniesInPeriod()
        {
            _folderBrowserDialog = new FolderBrowserDialog();
            ICollection<InsuranceCompany> allCompanies = Context
                .InsuranceCompany
                .ToList();
            _reportInsuranceCompanies = allCompanies
                .Where(c => c.Patient.Any(PatientsWithServicesInPeriod()))
                .ToList();
        }

        private void ExportInsuranceCompanyReport()
        {
            ExcelDrawingContext drawingContext = new ExcelDrawingContext();
            var drawer = new InsuranceCompanyContentDrawer(drawingContext,
                _folderBrowserDialog.SelectedPath,
                _reportInsuranceCompanies,
                FromPeriod,
                ToPeriod);
            new InsuranceCompanyCsvPdfExporter(drawer).Export();
            DateValidationErrors = "Отчёт успешно сформирован по пути " +
                _folderBrowserDialog.SelectedPath + "!";
        }

        private Func<Patient, bool> PatientsWithServicesInPeriod()
        {
            return p => p.AppliedService.Any(ServicesInPeriod());
        }

        private Func<AppliedService, bool> ServicesInPeriod()
        {
            return s => s.FinishedDateTime <= ToPeriod
                        && s.FinishedDateTime >= FromPeriod;
        }
    }
}
