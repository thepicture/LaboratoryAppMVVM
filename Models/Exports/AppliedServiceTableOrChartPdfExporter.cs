using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Modelss.Exports;
using System.Windows.Forms.DataVisualization.Charting;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class AppliedServiceTableOrChartPdfExporter : TableOrChartExporter
    {
        public AppliedServiceTableOrChartPdfExporter(
            Report report,
            string folderPath,
            Chart chart) : base(report, folderPath, chart)
        {
        }

        public override void ExportAsChart()
        {
            Chart.SaveImage(Buffer, ChartImageFormat.Png);
            WordDrawingContext wordDrawingContext = new WordDrawingContext();
            Drawer = new AppliedServiceChartDrawer(
                    wordDrawingContext,
                    FolderPath,
                    Buffer,
                    Report as AppliedServiceReport);
        }

        public override void ExportAsTable()
        {
            throw new System.NotImplementedException();
        }
    }
}
