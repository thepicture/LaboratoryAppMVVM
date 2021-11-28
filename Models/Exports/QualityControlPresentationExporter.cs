using LaboratoryAppMVVM.Models.Entities;
using System.Windows.Forms.DataVisualization.Charting;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class QualityControlPresentationExporter : PresentationExporter
    {
        private readonly Service _service;

        public QualityControlPresentationExporter(
            Report report,
            Chart chart,
            string exportType,
            Service service) : base(report, chart, exportType)
        {
            _service = service;
        }

        public override void ExportChartToPdf()
        {
            using (var exporter = new QualityControlTableOrChartPdfExporter(Report,
                 _selectedSavePath,
                 Chart))
            {
                exporter.ExportAsChart();
            }
        }

        public override void ExportTableToPdf()
        {
            using (var exporter = new QualityControlTableOrChartPdfExporter(Report,
                _selectedSavePath,
                Chart,
                _service
                ))
            {
                exporter.ExportAsTable();
            }
        }
    }
}
