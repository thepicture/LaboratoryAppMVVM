using LaboratoryAppMVVM.Models.Entities;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class OrderPdfExporter : Exporter<Order>
    {
        public OrderPdfExporter(ContentDrawer<Order> contentDrawer) : base(contentDrawer)
        {
        }

        public override void Export()
        {
            base.Export();
        }
    }
}
