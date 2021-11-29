using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using LaboratoryAppMVVM.Models.Exports;
using LaboratoryAppMVVM.Models.Generators;
using LaboratoryAppMVVM.Models.LaboratoryIO;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WIA;

namespace LaboratoryAppMVVM.ViewModels
{
    public class CreateOrEditOrderViewModel : ViewModelBase
    {
        private const int barcodeStartNumber = 100000;
        private const int barcodeEndNumber = 999999 + 1;
        private const int maxValueForBarcodeId = 100;
        private const int maxLevenshteinDistance = 4;
        private const int barcodeWidth = 200;
        private const int bacodeHeight = 40;
        private readonly ViewModelNavigationStore _navigationStore;
        private readonly ViewModelBase _laboratoryAssistantViewModel;
        private Order _order;
        private LaboratoryDatabaseEntities _context;
        private RelayCommand _navigateToLaboratoryAssistantViewModel;
        private RelayCommand _enterTooltipCommand;
        private RelayCommand _getBarcodeFromScannerCommand;
        private RelayCommand _navigateToAddPatientViewModelCommand;
        private string _tubeIdTooltipText = "Введите код пробирки...";
        private string _tubeId;
        private RenderTargetBitmap _barcodeBitmap;
        private List<Patient> _patients;
        private Patient _selectedPatient;
        private string _searchPatientText = "";
        private ObservableCollection<Service> _allServices;
        private ObservableCollection<Service> _orderServices;
        private RelayCommand _addServiceToOrderCommand;
        private RelayCommand _deleteServiceFromOrderCommand;
        private RelayCommand _addNewServiceCommand;
        private bool _isAddServicePanelVisible = false;
        private RelayCommand _showAddServiceFieldCommand;
        private string _searchServiceText = "";
        private readonly ICalculator _levenshteinDistanceCalculator;
        private RelayCommand _createOrderCommand;
        private string _barcodeText;
        private readonly IBrowserDialog _dialog;
        private NameValueCollection _orderBase64String;
        private DeviceInfo _scanner;
        private DeviceManager _deviceManager;

        public CreateOrEditOrderViewModel(ViewModelNavigationStore navigationStore,
                                          User user,
                                          Order order,
                                          IMessageService messageBoxService,
                                          ViewModelBase laboratoryAssistantViewModel)
        {
            _navigationStore = navigationStore;
            User = user;
            Order = order;
            _laboratoryAssistantViewModel = laboratoryAssistantViewModel;
            MessageService = messageBoxService;
            _levenshteinDistanceCalculator = new LevenshteinDistanceCalculator();
            _dialog = new SimpleFolderDialog();
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
                            _navigationStore.CurrentViewModel =
                            _laboratoryAssistantViewModel;
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
                string tempBarCodePath = GetBarcodePath();
                TryToSaveBarcodeToPdfFile(tempBarCodePath);
            }
        }

        private void TryToSaveBarcodeToPdfFile(string tempBarCodePath)
        {
            try
            {
                SaveBarcodeToPdfFile(tempBarCodePath);
            }
            catch (PdfExportException ex)
            {
                MessageService.ShowError("Не удалось " +
                    "сохранить штрих-код. " +
                    "Пожалуйста, попробуйте сохранить " +
                    "файл ещё раз. " +
                    "Ошибка: " + ex.Message);
            }
            finally
            {
                DeleteTempBarcodeFileIfItExists(tempBarCodePath);
            }
        }

        private static void DeleteTempBarcodeFileIfItExists(string tempBarCodePath)
        {
            if (File.Exists(tempBarCodePath))
            {
                File.Delete(tempBarCodePath);
            }
        }

        private string GetBarcodePath()
        {
            string tempBarCodePath = Path.Combine(AppDomain
            .CurrentDomain
            .BaseDirectory, "tempBarcode.png");
            _barcodeText = $"{TubeId}" +
                                $"{DateTime.Now:ddMMyyyy}" +
                                $"{GetBarcodeNumber()}";
            return tempBarCodePath;
        }

        private static int GetBarcodeNumber()
        {
            return new Random().Next(barcodeStartNumber, barcodeEndNumber);
        }

        private void SaveBarcodeToPdfFile(string tempBarCodePath)
        {
            GenerateBarcodeBitmap(tempBarCodePath);
            if (!_dialog.ShowDialog())
            {
                return;
            }
            else
            {
                SaveBarcodeToSystem(tempBarCodePath);
                SetClipboard();
            }
        }

        private void SetClipboard()
        {
            Clipboard.SetText(_dialog.GetSelectedItem() as string);
        }

        private void SaveBarcodeToSystem(string tempBarCodePath)
        {
            BarcodeContentDrawer contentDrawer = DrawContent(tempBarCodePath);
            new Exporter(contentDrawer).Export();

            MessageService.ShowInformation("Документ " +
                "успешно сохранён по пути " +
                _dialog.GetSelectedItem() as string +
                "Путь скопирован в буфер обмена");
        }

        private BarcodeContentDrawer DrawContent(string tempBarCodePath)
        {
            IDrawingContext drawingContext = new WordDrawingContext();
            BarcodeContentDrawer contentDrawer =
                new BarcodeContentDrawer(drawingContext,
                                         _dialog.GetSelectedItem() as string,
                                         new Barcode(tempBarCodePath));
            return contentDrawer;
        }

        private void GenerateBarcodeBitmap(string tempBarCodePath)
        {
            BarcodeBitmap = new BarcodeImageGenerator(_barcodeText)
                            .Generate(new Size(barcodeWidth, bacodeHeight));
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(BarcodeBitmap));
            using (FileStream stream = new FileStream(
                tempBarCodePath,
                FileMode.Create))
            {
                encoder.Save(stream);
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
                                MessageService,
                                this
                                )
                            );
                }
                return _navigateToAddPatientViewModelCommand;
            }
        }

        public List<Patient> Patients
        {
            get
            {
                if (_patients == null)
                {
                    _patients = Context.Patient.ToList();
                }
                return _patients;
            }

            set
            {
                _patients = value;
                OnPropertyChanged();
            }
        }
        public Patient SelectedPatient
        {
            get
            {
                if (_selectedPatient == null)
                {
                    _selectedPatient = Patients.FirstOrDefault();
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

        public ObservableCollection<Service> AllServices
        {
            get
            {
                if (_allServices == null)
                {
                    _allServices = new ObservableCollection<Service>(Context.Service);
                }
                return _allServices;
            }

            set
            {
                _allServices = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Service> OrderServices
        {
            get
            {
                if (_orderServices == null)
                {
                    _orderServices = new ObservableCollection<Service>();
                }
                return _orderServices;
            }

            set
            {
                _orderServices = value;
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
                            OrderServices.Add(param as Service);
                            _ = AllServices
                            .Remove(AllServices.First(s => s.Id
                                                           == (param as Service).Id));
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
                            AllServices.Add(param as Service);
                            _ = OrderServices
                            .Remove(OrderServices.First(s => s.Id == (param
                                                                      as Service).Id));
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
            InsertValuesIntoOrder();
            if (!Context.BarcodeOfPatient.Any(barcode => barcode.Barcode == TubeId))
            {
                CreateNewBarcodeAndAssignItToOrder();
            }
            else
            {
                Order.BarcodeOfPatient = Context
                    .BarcodeOfPatient
                    .First(barcode => barcode.Barcode == TubeId);
            }
            _ = Context.Order.Add(Order);
            CreateNameValueCollectionOfOrder();
            TryToSaveOrder();
        }

        private void TryToSaveOrder()
        {
            try
            {
                _ = Context.SaveChanges();
                string nameOfOrder = $"Заказ_{Order.Date:yyyy_MM_dd_hh_mm_ss}";

                if (_dialog.ShowDialog())
                {
                    SaveOrderInfo();
                }
                else
                {
                    MessageService.ShowInformation("Заказ успешно "
                        + "сохранён в базу данных "
                        + "без отчётности!");
                }
            }
            catch (Exception ex)
            {
                MessageService.ShowError("Не удалось сформировать "
                    + "заказ. Пожалуйста, попробуйте ещё раз. "
                    + "Ошибка: "
                    + ex.Message);
            }
        }

        private void SaveOrderInfo()
        {
            ExportOrderPdf();
            ExportOrderBase64();
            MessageService.ShowInformation("Информация о заказе "
                + "успешно сохранена в базу данных, а также "
                + $"по пути {_dialog.GetSelectedItem() as string} "
                + $"в формате .pdf и .txt!");
        }

        private void ExportOrderBase64()
        {
            string urlEncodeText = $"https://wsrussia.ru/" +
                $"?data=base64({_orderBase64String})";
            File.WriteAllText(Path.Combine(
                _dialog.GetSelectedItem() as string,
                "заказ" + ".txt"),
                urlEncodeText,
                encoding: System.Text.Encoding.UTF8);
        }

        private void ExportOrderPdf()
        {
            IDrawingContext drawingContext = new WordDrawingContext();
            OrderContentDrawer contentDrawer =
                new OrderContentDrawer(drawingContext,
                                       _dialog.GetSelectedItem() as string,
                                       Order);
            new Exporter(contentDrawer).Export();
        }

        private void CreateNameValueCollectionOfOrder()
        {
            _orderBase64String = System.Web.HttpUtility.ParseQueryString(string.Empty);
            foreach (System.Reflection.PropertyInfo property in Order.GetType()
                .GetProperties())
            {
                _orderBase64String.Add("дата_заказа",
                                      _order.Date.ToString("yyyy-MM-dd_hh:mm:ss"));
                _orderBase64String.Add("номер_заказа",
                                      _order.Id.ToString());
                _orderBase64String.Add("номер_пробирки",
                                      _order.BarcodeOfPatient.Barcode);
                _orderBase64String.Add("номер_страхового полиса",
                                      _order.Patient.InsurancePolicyNumber
                                      ?? "Не указан");
                _orderBase64String.Add("фио",
                                      _order.Patient.FullName);
                _orderBase64String.Add("дата_рождения",
                                      _order.Patient.BirthDate.ToString("yyyy-MM-dd"));
                _orderBase64String.Add("перечень_услуг",
                                      string.Join(", ", _order.AppliedService.ToList()
                                      .Select(s => s.Service.Name)));
                _orderBase64String.Add("стоимость",
                                      _order.AppliedService.Sum(s => s.Service.Price)
                                      .ToString("N2"));
            }
        }

        private void CreateNewBarcodeAndAssignItToOrder()
        {
            Order.BarcodeOfPatient = new BarcodeOfPatient
            {
                DateTime = DateTime.Now,
                Barcode = TubeId
            };
        }

        private void InsertValuesIntoOrder()
        {
            OrderServices
                            .Select(orderService =>
                            {
                                return ConvertServiceToAppliedService(orderService);
                            })
                            .ToList()
                            .ForEach(Order.AppliedService.Add);
            Order.Patient = SelectedPatient;
            Order.Date = DateTime.Now;
            Order.StatusOfOrder = Context.StatusOfOrder
                .First(status => status.Name == "В обработке");
        }

        private AppliedService ConvertServiceToAppliedService(Service orderService)
        {
            return new AppliedService
            {
                AnalyzerId = _context.Analyzer.First().Id,
                Result = default,
                ServiceId = orderService.Id,
                FinishedDateTime = DateTime.Now + TimeSpan.FromDays(1),
                IsAccepted = false,
                StatusId = _context.StatusOfAppliedService
                .First(statusOfAppliedService => statusOfAppliedService.Name
                .StartsWith("В обработке")).Id,
                UserId = User.Id,
                PatientId = SelectedPatient.Id
            };
        }

        private void AddNewService(string serviceName)
        {
            if (IsServiceAlreadyExists(serviceName))
            {
                MessageService.ShowError("Не удалось добавить " +
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
                MessageService.ShowInformation("Услуга " +
                    "успешно добавлена!");
                AllServices.Add(newService);
                IsAddServicePanelVisible = false;
            }
            catch (Exception ex)
            {
                MessageService.ShowError("Не удалось добавить услугу. " +
                    "Пожалуйста, попробуйте ещё раз. " +
                    "Ошибка: " + ex.Message);
            }
        }

        private bool IsServiceAlreadyExists(string serviceName)
        {
            return Context.Service.Any(service => service.Name.ToLower()
            .Contains(serviceName));
        }

        private void FilterAllPatients()
        {
            List<Patient> currentPatients = Context.Patient.ToList();
            if (!string.IsNullOrWhiteSpace(SearchPatientText))
            {
                currentPatients = currentPatients
                    .Where(new AllPropertiesSearcher(SearchPatientText)
                    .Search<Patient>())
                    .ToList();
            }
            currentPatients = currentPatients
                .Union(Context.Patient.ToList()
                .Where(p => (int)_levenshteinDistanceCalculator.Calculate(
                p.FullName.ToLower(),
                SearchPatientText.ToLower()) < maxLevenshteinDistance)).ToList();
            Patients = currentPatients;
            SelectedPatient = currentPatients.FirstOrDefault();
        }

        private void FilterAllServices()
        {
            List<Service> currentServices = Context.Service.ToList();
            if (!string.IsNullOrWhiteSpace(SearchServiceText))
            {
                currentServices = currentServices
                    .Where(new AllPropertiesSearcher(SearchServiceText)
                    .Search<Service>())
                    .ToList();
            }
            currentServices = currentServices
                .Union(Context.Service.ToList()
                .Where(p => (int)_levenshteinDistanceCalculator.Calculate(
                    p.Name.ToLower(),
                    SearchServiceText.ToLower()) < maxLevenshteinDistance))
                .ToList();
            AllServices.Clear();
            AllServices = new ObservableCollection<Service>
                (
                currentServices
                .Where(service => !OrderServices.Select(orderService => orderService.Id)
                .Contains(service.Id))
                .ToList()
                );
        }


        private void GetBarcodeForScanner()
        {
            _deviceManager = new DeviceManager();
            FindFirstAvailableScanner();
            if (_scanner == null)
            {
                MessageService.ShowError("Сканеры " +
                    "не обнаружены " +
                    "на вашей рабочей системе. " +
                    "Пожалуйста, проверьте " +
                    "подключение сканера " +
                    "к вашему USB-порту");
                return;
            }

            ParseTubeIdFromScanner();
        }

        private void ParseTubeIdFromScanner()
        {
            Device device = _scanner.Connect();
            Item item = device.Items[1];
            ImageFile imageFile = (ImageFile)item
                .Transfer(FormatID.wiaFormatPNG);
            string barcode = ParseCodeFromImageFile(imageFile);
            TubeId = barcode.Replace("\r", "");
        }

        private void FindFirstAvailableScanner()
        {
            for (int i = 1; i < _deviceManager.DeviceInfos.Count; i++)
            {
                if (_deviceManager.DeviceInfos[i].Type
                    != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }
                _scanner = _deviceManager.DeviceInfos[i];
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style",
            "IDE0060:Remove unused parameter",
            Justification = "Method will be extended later.")]
        private string ParseCodeFromImageFile(ImageFile imageFile)
        {
            return $"{new Random().Next(maxValueForBarcodeId)}"
                + $"{DateTime.Now:ddMMyyyy}"
                + $"{new Random().Next(GetBarcodeNumber())}";
        }

        private RelayCommand editPatientCommand;

        public ICommand EditPatientCommand
        {
            get
            {
                if (editPatientCommand == null)
                {
                    editPatientCommand = new RelayCommand(EditPatient, () => SelectedPatient != null);
                }

                return editPatientCommand;
            }
        }

        private void EditPatient(object commandParameter)
        {
            _navigationStore.CurrentViewModel = new AddPatientViewModel(_navigationStore,
                MessageService,
                this,
                SelectedPatient);
        }
    }
}
