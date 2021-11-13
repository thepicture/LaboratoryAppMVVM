using LaboratoryAppMVVM.Commands;
using LaboratoryAppMVVM.Models;
using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using LaboratoryAppMVVM.Services;
using LaboratoryAppMVVM.Stores;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

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
        private RenderTargetBitmap _barcodeBitmap;

        public CreateOrEditOrderView(ViewModelNavigationStore navigationStore, User user, Order order, IMessageBoxService messageBoxService)
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
                        else
                        {
                            string tempBarCodePath = AppDomain
                            .CurrentDomain
                            .BaseDirectory + "tempBarcode.png";
                            BarcodeBitmap = new BarcodeImageGenerator($"{TubeId}" +
                                $"{DateTime.Now.Day}" +
                                $"{DateTime.Now.Month}" +
                                $"{DateTime.Now.Year}" +
                                $"{new Random().Next(100000, 999999 + 1)}").Generate(200, 40);
                            PngBitmapEncoder encoder = new PngBitmapEncoder();
                            try
                            {
                                encoder.Frames.Add(BitmapFrame.Create(BarcodeBitmap));
                                using (FileStream stream = new FileStream(tempBarCodePath, FileMode.Create))
                                {
                                    encoder.Save(stream);
                                }
                                string filePath = new CustomPathPdfExporter(new BarcodePdfExporter())
                                .Save(isShowAfterSave: true);
                                MessageBoxService.ShowInformation("Документ успешно сохранён по пути " +
                                    filePath +
                                    "Путь скопирован в буфер обмена");
                                Clipboard.SetText(filePath);
                            }
                            catch (PdfExportException ex)
                            {
                                MessageBoxService.ShowError("Не удалось сохранить штрих-код. " +
                                    "Пожалуйста, попробуйте сохранить файл ещё раз. " +
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

        public RenderTargetBitmap BarcodeBitmap
        {
            get => _barcodeBitmap; set
            {
                _barcodeBitmap = value;
                OnPropertyChanged();
            }
        }
    }
}
