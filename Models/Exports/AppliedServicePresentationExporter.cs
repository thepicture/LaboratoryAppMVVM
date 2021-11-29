using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.LaboratoryIO;
using System.Windows.Forms.DataVisualization.Charting;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class AppliedServicePresentationExporter : PresentationExporter
    {
        public AppliedServicePresentationExporter(
            Report report,
            Chart chart,
            string exportType,
            IBrowserDialog dialog) : base(report, chart, exportType, dialog)
        {
        }

        public override void ExportChartToPdf()
        {
            using (AppliedServiceTableOrChartPdfExporter exporter
                = new AppliedServiceTableOrChartPdfExporter(_report,
                 _selectedSavePath,
                 Chart))
            {
                exporter.ExportAsChart();
            }
        }

        public override void ExportTableToPdf()
        {
            using (AppliedServiceTableOrChartPdfExporter exporter =
                new AppliedServiceTableOrChartPdfExporter(_report,
                 _selectedSavePath,
                 Chart))
            {
                exporter.ExportAsTable();
            }
        }
    }
}
