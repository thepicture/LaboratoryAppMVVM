using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Stores;
using System.Collections.ObjectModel;
using System.Linq;

namespace LaboratoryAppMVVM.ViewModels
{
    public class AnalyzerViewModel : ViewModelBase
    {
        private readonly ViewModelNavigationStore _viewModelNavigationStore;
        private Analyzer _analyzer;
        private ObservableCollection<AppliedService> _notAcceptedServicesList;

        public AnalyzerViewModel(ViewModelNavigationStore viewModelNavigationStore,
                                 Analyzer analyzer)
        {
            _viewModelNavigationStore = viewModelNavigationStore;
            Analyzer = analyzer;
        }

        public Analyzer Analyzer
        {
            get => _analyzer; set
            {
                _analyzer = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<AppliedService> NotAcceptedServicesList
        {
            get
            {
                if (_notAcceptedServicesList == null)
                {
                    _notAcceptedServicesList = new ObservableCollection<AppliedService>
                        (
                            Analyzer.AppliedService.Where(s => !s.IsAccepted)
                        );
                }
                return _notAcceptedServicesList;
            }

            set
            {
                _notAcceptedServicesList = value;
                OnPropertyChanged();
            }
        }
    }
}
