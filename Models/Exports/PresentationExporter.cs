using LaboratoryAppMVVM.Models.Entities;
using System;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace LaboratoryAppMVVM.Models.Exports
{
    public abstract class PresentationExporter : IExporter
    {
        protected readonly Report _report;
        private readonly Chart _chart;
        private readonly string _exportType;
        protected string _selectedSavePath;

        public string SelectedSavePath { get => _selectedSavePath; set => _selectedSavePath = value; }

        public Report Report => _report;

        public Chart Chart => _chart;

        public PresentationExporter(Report report, Chart chart, string exportType)
        {
            _report = report;
            _chart = chart;
            _exportType = exportType;
        }

        public void Export()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            SelectedSavePath = folderBrowserDialog.SelectedPath;
            if (Chart == null)
            {
                throw new NullReferenceException("Экспорт неуспешен, потому что "
                                     + "график недоступен. "
                                     + "Пожалуйста, перезайдите на страницу"
                                     + "и попробуйте ещё раз");
            }
            switch (_exportType)
            {
                case "только график":
                    ExportChartToPdf();
                    break;
                case "только таблица":
                    ExportTableToPdf();
                    break;
                case "график и таблица":
                    ExportChartToPdf();
                    ExportTableToPdf();
                    break;
                default:
                    break;
            }
        }

        public abstract void ExportTableToPdf();

        public abstract void ExportChartToPdf();
    }
}
