using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exports;
using System;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace LaboratoryAppMVVM.Modelss.Exports
{
    public abstract class TableOrChartExporter :
        ITableExporter, IChartExporter, IDisposable
    {
        public MemoryStream Buffer;
        protected TableOrChartExporter(Report report,
                                       string folderPath,
                                       Chart chart)
        {
            Report = report;
            FolderPath = folderPath;
            Chart = chart;
            Buffer = new MemoryStream();
        }

        public Report Report { get; }
        public string FolderPath { get; }
        public Chart Chart { get; }
        public ContentDrawer Drawer { get; set; }

        public void Dispose()
        {
            Buffer.Dispose();
            new Exporter(Drawer).Export();
        }

        public abstract void ExportAsChart();

        public abstract void ExportAsTable();
    }
}
