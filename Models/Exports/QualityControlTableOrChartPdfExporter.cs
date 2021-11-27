using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Modelss.Exports;
using System.Windows.Forms.DataVisualization.Charting;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class QualityControlTableOrChartPdfExporter : TableOrChartExporter
    {
        public QualityControlTableOrChartPdfExporter(
            Report report,
            string folderPath,
            Chart chart,
            Service service = null) : base(report, folderPath, chart)
        {
            Service = service;
        }

        public Service Service { get; }

        public override void ExportAsChart()
        {
            Chart.SaveImage(Buffer, ChartImageFormat.Png);
            WordDrawingContext wordDrawingContext = new WordDrawingContext();
            Drawer
                = new QualityControlChartDrawer(
                    wordDrawingContext,
                    FolderPath,
                    Buffer);
        }

        public override void ExportAsTable()
        {
            WordDrawingContext wordDrawingContext = new WordDrawingContext();
            Drawer
                = new QualityControlTableDrawer(
                    wordDrawingContext,
                    FolderPath,
                    new QualityControlReport(Service));
        }
    }
}
