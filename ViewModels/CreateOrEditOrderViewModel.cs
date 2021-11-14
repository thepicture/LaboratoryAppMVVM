﻿using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using WIA;

namespace LaboratoryAppMVVM.ViewModels
{
    public class CreateOrEditOrderViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private Order _order;
        private LaboratoryDatabaseEntities _context;
        private RelayCommand _navigateToLaboratoryAssistantViewModel;
        private RelayCommand _enterTooltipCommand;
        private RelayCommand _getBarcodeFromScannerCommand;
        private RelayCommand _navigateToAddPatientViewModelCommand;
        private string _tubeIdTooltipText = "Введите код пробирки...";
        private string _tubeId;
        private RenderTargetBitmap _barcodeBitmap;
        private List<Patient> _patientsList;
        private Patient _selectedPatient;
        private string _searchPatientText = "";
        private ObservableCollection<Service> _allServicesList;
        private ObservableCollection<Service> _orderServicesList;
        private RelayCommand _addServiceToOrderCommand;
        private RelayCommand _deleteServiceFromOrderCommand;

        public CreateOrEditOrderViewModel(ViewModelNavigationStore navigationStore, User user, Order order, IMessageBoxService messageBoxService)
        {
            _navigationStore = navigationStore;
            User = user;
            Order = order;
            MessageBoxService = messageBoxService;
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

        public RelayCommand NavigateToLaboratoryAssistantViewModel
        {
            get
            {
                if (_navigateToLaboratoryAssistantViewModel
                    == null)
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
                    _tubeIdTooltipText = Convert.ToString(
                        Context.Order.ToList().Last().Id + 1
                        );
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
                    _enterTooltipCommand =
                        new RelayCommand(param => GeneratePdfBarcode());
                }
                return _enterTooltipCommand;
            }
        }

        private void GeneratePdfBarcode()
        {
            if (string.IsNullOrWhiteSpace(TubeId))
            {
                TubeId = TubeIdTooltipText;
            }
            else
            {
                string tempBarCodePath = AppDomain
                .CurrentDomain
                .BaseDirectory + "tempBarcode.png";
                string barcodeText = $"{TubeId}" +
                                    $"{DateTime.Now:ddMMyyyy}" +
                                    $"{new Random().Next(100000, 999999 + 1)}";
                try
                {
                    SaveBarcodeToPdfFile(tempBarCodePath, barcodeText);
                }
                catch (PdfExportException ex)
                {
                    MessageBoxService.ShowError("Не удалось " +
                        "сохранить штрих-код. " +
                        "Пожалуйста, попробуйте сохранить " +
                        "файл ещё раз. " +
                        "Ошибка: " + ex.Message);
                }
                finally
                {
                    if (File.Exists(tempBarCodePath))
                    {
                        File.Delete(tempBarCodePath);
                    }
                }
            }
        }

        private void SaveBarcodeToPdfFile(string tempBarCodePath,
                                          string barcodeText)
        {
            BarcodeBitmap = new BarcodeImageGenerator(
    barcodeText)
    .Generate(200, 40);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(BarcodeBitmap));
            using (FileStream stream = new FileStream(
                tempBarCodePath,
                FileMode.Create))
            {
                encoder.Save(stream);
            }
            string filePath = new CustomPathPdfExporter(
                pdfExportable: new BarcodePdfExporter()
                )
            .Save(isShowAfterSave: true);
            bool fileWasSaved = !string.IsNullOrWhiteSpace(
                filePath);

            if (fileWasSaved)
            {
                MessageBoxService.ShowInformation("Документ " +
                    "успешно сохранён по пути " +
                    filePath +
                    "Путь скопирован в буфер обмена");
                Clipboard.SetText(filePath);
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

        public RenderTargetBitmap BarcodeBitmap
        {
            get => _barcodeBitmap; set
            {
                _barcodeBitmap = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand GetBarcodeFromScannerCommand
        {
            get
            {
                if (_getBarcodeFromScannerCommand == null)
                {
                    _getBarcodeFromScannerCommand =
                        new RelayCommand(param =>
                        GetBarcodeForScanner());
                }
                return _getBarcodeFromScannerCommand;
            }
        }

        public RelayCommand NavigateToAddPatientViewModelCommand
        {
            get
            {
                if (_navigateToAddPatientViewModelCommand == null)
                {
                    _navigateToAddPatientViewModelCommand =
                        new RelayCommand(param => _navigationStore.CurrentViewModel =
                            new AddPatientViewModel(
                                _navigationStore,
                                MessageBoxService,
                                this
                                )
                            );
                }
                return _navigateToAddPatientViewModelCommand;
            }
        }

        public List<Patient> PatientsList
        {
            get
            {
                if (_patientsList == null)
                {
                    _patientsList = Context.Patient.ToList();
                }
                return _patientsList;
            }

            set
            {
                _patientsList = value;
                OnPropertyChanged();
            }
        }
        public Patient SelectedPatient
        {
            get
            {
                if (_selectedPatient == null)
                {
                    _selectedPatient = PatientsList.FirstOrDefault();
                }
                return _selectedPatient;
            }

            set
            {
                _selectedPatient = value;
                OnPropertyChanged();
            }
        }

        public string SearchPatientText
        {
            get => _searchPatientText; set
            {
                _searchPatientText = value;
                FilterPatients();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Service> AllServicesList
        {
            get
            {
                if (_allServicesList == null)
                {
                    _allServicesList = new ObservableCollection<Service>(Context.Service);
                }
                return _allServicesList;
            }

            set
            {
                _allServicesList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Service> OrderServicesList
        {
            get
            {
                if (_orderServicesList == null)
                {
                    _orderServicesList = new ObservableCollection<Service>();
                }
                return _orderServicesList;
            }

            set
            {
                _orderServicesList = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AddServiceToOrderCommand
        {
            get
            {
                if (_addServiceToOrderCommand == null)
                {
                    _addServiceToOrderCommand =
                        new RelayCommand(param =>
                        {
                            OrderServicesList.Add(param as Service);
                            AllServicesList.Remove(AllServicesList.Where(s => s.Id == (param as Service).Id).First());
                        });
                }
                return _addServiceToOrderCommand;
            }
        }

        public RelayCommand DeleteServiceFromOrderCommand
        {
            get
            {
                if (_deleteServiceFromOrderCommand == null)
                {
                    _deleteServiceFromOrderCommand =
                        new RelayCommand(param =>
                        {
                            AllServicesList.Add(param as Service);
                            OrderServicesList.Remove(OrderServicesList.Where(s => s.Id == (param as Service).Id).First());
                        });
                }
                return _deleteServiceFromOrderCommand;
            }
        }

        private void FilterPatients()
        {
            List<Patient> currentPatients = Context.Patient.ToList();
            if (!string.IsNullOrWhiteSpace(SearchPatientText))
            {
                currentPatients = currentPatients
                    .Where(new AllPropertiesSearcher<Patient>().Search(SearchPatientText))
                    .ToList();
            }
            PatientsList = currentPatients;
            SelectedPatient = currentPatients.FirstOrDefault();
        }

        private void GetBarcodeForScanner()
        {
            DeviceManager deviceManager = new DeviceManager();
            DeviceInfo scanner = null;

            for (int i = 1;
                     i < deviceManager.DeviceInfos.Count;
                     i++)
            {
                if (deviceManager.DeviceInfos[i].Type
                    != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }

                scanner = deviceManager.DeviceInfos[i];
            }

            if (scanner == null)
            {
                MessageBoxService.ShowError("Сканеры " +
                    "не обнаружены " +
                    "на вашей рабочей системе. " +
                    "Пожалуйста, проверьте " +
                    "подключение сканера " +
                    "к вашему USB-порту");
                return;
            }

            Device device = scanner.Connect();
            Item item = device.Items[1];
            ImageFile imageFile = (ImageFile)item
                .Transfer(FormatID.wiaFormatPNG);
            string barcode = ParseCodeFromImageFile(imageFile);
            TubeId = barcode.Replace("\r", "");

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Method will be extended later.")]
        private string ParseCodeFromImageFile(ImageFile imageFile)
        {
            return $"{new Random().Next(100)}"
                + $"{DateTime.Now:ddMMyyyy}"
                + $"{new Random().Next(100000, 999999 + 1)}";
        }
    }
}
