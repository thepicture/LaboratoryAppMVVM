namespace LaboratoryAppMVVM.Models.Exports
{
    class BarcodePdfExporter : Exporter<Barcode>
    {
        public BarcodePdfExporter(ContentDrawer<Barcode> contentDrawer) : base(contentDrawer)
        {
        }

        public override void Export()
        {
            base.Export();
        }
    }
}
