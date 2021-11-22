using LaboratoryAppMVVM.Models.Entities;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class BarcodePdfExporter : Exporter<Barcode>
    {
        public BarcodePdfExporter(ContentDrawer<Barcode> contentDrawer) : base(contentDrawer)
        {
        }
    }
}
