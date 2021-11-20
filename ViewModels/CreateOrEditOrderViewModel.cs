using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WIA;

namespace LaboratoryAppMVVM.ViewModels
{
    public class CreateOrEditOrderViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly LaboratoryAssistantViewModel _laboratoryAssistantViewModel;
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
        private RelayCommand _addNewServiceCommand;
        private bool _isAddServicePanelVisible = false;
        private RelayCommand _showAddServiceFieldCommand;
        private string _searchServiceText = "";
        private readonly LevenshteinDistanceCalculator _levenshteinDistanceCalculator;
        private RelayCommand _createOrderCommand;

        public CreateOrEditOrderViewModel(ViewModelNavigationStore navigationStore,
                                          User user,
                                          Order order,
                                          IMessageBoxService messageBoxService,
                                          LaboratoryAssistantViewModel laboratoryAssistantViewModel)
        {
            _navigationStore = navigationStore;
            User = user;
            Order = order;
            _laboratoryAssistantViewModel = laboratoryAssistantViewModel;
            MessageBoxService = messageBoxService;
            _levenshteinDistanceCalculator = new LevenshteinDistanceCalculator();
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
                        new RelayCommand(param =>
                        {
                            _navigationStore.CurrentViewModel = _laboratoryAssistantViewModel;
                        });
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
                FilterAllPatients();
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
                            _ = AllServicesList.Remove(AllServicesList.First(s => s.Id == (param as Service).Id));
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
                            _ = OrderServicesList.Remove(OrderServicesList.First(s => s.Id == (param as Service).Id));
                        });
                }
                return _deleteServiceFromOrderCommand;
            }
        }

        public RelayCommand AddNewServiceCommand
        {
            get
            {
                if (_addNewServiceCommand == null)
                {
                    _addNewServiceCommand = new RelayCommand(param =>
                    {
                        AddNewService(param as string);
                    });
                }
                return _addNewServiceCommand;
            }
        }

        public bool IsAddServicePanelVisible
        {
            get => _isAddServicePanelVisible; set
            {
                _isAddServicePanelVisible = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ShowAddServiceFieldCommand
        {
            get
            {
                if (_showAddServiceFieldCommand == null)
                {
                    _showAddServiceFieldCommand = new RelayCommand(param =>
                    {
                        IsAddServicePanelVisible = true;
                    });
                }
                return _showAddServiceFieldCommand;
            }
        }

        public string SearchServiceText
        {
            get => _searchServiceText; set
            {
                _searchServiceText = value;
                FilterAllServices();
                OnPropertyChanged();
            }
        }

        public RelayCommand CreateOrderCommand
        {
            get
            {
                if (_createOrderCommand == null)
                {
                    _createOrderCommand = new RelayCommand(param => CreateOrder());
                }
                return _createOrderCommand;
            }
        }

        private void CreateOrder()
        {
            OrderServicesList.Select(s =>
            {
                return new AppliedService
                {
                    AnalyzerId = _context.Analyzer.First().Id,
                    Result = 0,
                    ServiceId = s.Id,
                    FinishedDateTime = DateTime.Now + TimeSpan.FromDays(1),
                    IsAccepted = false,
                    StatusId = _context.StatusOfAppliedService.First(status => status.Name.StartsWith("В обработке")).Id,
                    UserId = User.Id,
                    PatientId = SelectedPatient.Id
                };
            }).ToList().ForEach(Order.AppliedService.Add);
            Order.Patient = SelectedPatient;
            Order.Date = DateTime.Now;
            Order.StatusOfOrder = Context.StatusOfOrder.First(status => status.Name == "В обработке");
            if (!Context.BarcodeOfPatient.Any(barcode => barcode.Barcode == TubeId))
            {
                Order.BarcodeOfPatient = new BarcodeOfPatient
                {
                    DateTime = DateTime.Now,
                    Barcode = TubeId
                };
            }
            else
            {
                Order.BarcodeOfPatient = Context.BarcodeOfPatient.First(barcode => barcode.Barcode == TubeId);
            }
            _ = Context.Order.Add(Order);
            NameValueCollection orderBase64String = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (System.Reflection.PropertyInfo property in Order.GetType().GetProperties())
            {
                orderBase64String.Add("дата_заказа", _order.Date.ToString("yyyy-MM-ddThh:mm:ss"));
                orderBase64String.Add("номер_заказа", _order.Id.ToString());
                orderBase64String.Add("номер_пробирки", _order.BarcodeOfPatient.Barcode);
                orderBase64String.Add("номер_страхового полиса", _order.Patient.InsurancePolicyNumber ?? "Не указан");
                orderBase64String.Add("фио", _order.Patient.FullName);
                orderBase64String.Add("дата_рождения", _order.Patient.BirthDate.ToString("yyyy-MM-dd"));
                orderBase64String.Add("перечень_услуг", string.Join(", ", _order.AppliedService.ToList().Select(s => s.Service.Name)));
                orderBase64String.Add("стоимость", _order.AppliedService.Sum(s => s.Service.Price).ToString("N2"));
            }
            try
            {
                _ = Context.SaveChanges();
                string nameOfOrder = $"Заказ_{Order.Date:yyyy_MM_dd_hh_mm_ss}";
                CustomPathPdfExporter customPathPdfExporter = new CustomPathPdfExporter
                    (
                        new OrderPdfExporter(Order, nameOfOrder + ".pdf")
                    );
                string filePath = customPathPdfExporter.Save();
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    string urlEncodeText = $"https://wsrussia.ru/?data=base64({orderBase64String})";
                    File.WriteAllText(Path.Combine(filePath, nameOfOrder + ".txt"),
                        urlEncodeText,
                        encoding: System.Text.Encoding.UTF8);
                    MessageBoxService.ShowInformation("Информация о заказе " +
                        "успешно сохранена в базу данных, а также " +
                        $"по пути {filePath} в формате .pdf и .txt!");
                }
                else
                {
                    MessageBoxService.ShowInformation("Заказ успешно " +
                        "сохранён в базу данных " +
                        "с отчётностью в папке выполняемого " +
                        "в данный момент приложения!");
                }
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError("Не удалось сформировать " +
                    "заказ. Пожалуйста, попробуйте ещё раз. " +
                  "Ошибка: " + ex.Message);
            }
        }

        private void AddNewService(string serviceName)
        {
            if (Context.Service.Any(service => service.Name.ToLower().Contains(serviceName)))
            {
                MessageBoxService.ShowError("Не удалось добавить " +
                    "новую услугу в базу данных. " +
                    "Такая услуга уже существует. " +
                    "Пожалуйста, используйте поиск " +
                    "для выбора услуги, которую вы попытались " +
                    "добавить или проверьте наименование " +
                    "услуги");
                return;
            }

            Service newService = new Service { Name = serviceName };
            _ = Context.Service.Add(newService);
            try
            {
                _ = Context.SaveChanges();
                MessageBoxService.ShowInformation("Услуга " +
                    "успешно добавлена!");
                AllServicesList.Add(newService);
                IsAddServicePanelVisible = false;
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowError("Не удалось добавить услугу. " +
                    "Пожалуйста, попробуйте ещё раз. " +
                    "Ошибка: " + ex.Message);
            }
        }

        private void FilterAllPatients()
        {
            List<Patient> currentPatients = Context.Patient.ToList();
            if (!string.IsNullOrWhiteSpace(SearchPatientText))
            {
                currentPatients = currentPatients
                    .Where(new AllPropertiesSearcher(SearchPatientText).Search<Patient>())
                    .ToList();
            }
            currentPatients = currentPatients
                .Union(Context.Patient.ToList()
                .Where(p => _levenshteinDistanceCalculator.Calculate(
                p.FullName.ToLower(),
                SearchPatientText.ToLower()) < 4)).ToList();
            PatientsList = currentPatients;
            SelectedPatient = currentPatients.FirstOrDefault();
        }

        private void FilterAllServices()
        {
            List<Service> currentServices = Context.Service.ToList();
            if (!string.IsNullOrWhiteSpace(SearchServiceText))
            {
                currentServices = currentServices
                    .Where(new AllPropertiesSearcher(SearchServiceText).Search<Service>())
                    .ToList();
            }
            currentServices = currentServices
                .Union(Context.Service.ToList()
                .Where(p => _levenshteinDistanceCalculator.Calculate(
                    p.Name.ToLower(),
                    SearchServiceText.ToLower()) < 4))
                .ToList();
            AllServicesList.Clear();
            AllServicesList = new ObservableCollection<Service>
                (
                currentServices
                .Where(service => !OrderServicesList.Select(orderService => orderService.Id)
                .Contains(service.Id))
                .ToList()
                );
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
