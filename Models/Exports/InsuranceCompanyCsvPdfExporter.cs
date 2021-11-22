using LaboratoryAppMVVM.Models.Entities;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class InsuranceCompanyCsvPdfExporter :
        Exporter<InsuranceCompany>
    {
        public InsuranceCompanyCsvPdfExporter(ContentDrawer<InsuranceCompany> contentDrawer) : base(contentDrawer)
        {
        }
    }
}
