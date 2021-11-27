namespace LaboratoryAppMVVM.Models.Exports
{
    /// <summary>
    /// Defines a method 
    /// for exporting data 
    /// from one representation to another.
    /// </summary>
    public class Exporter : IExporter
    {
        protected readonly ContentDrawer _contentDrawer;

        public Exporter(ContentDrawer contentDrawer)
        {
            _contentDrawer = contentDrawer;
        }

        /// <summary>
        /// Exports data from one representation 
        /// to another.
        /// </summary>
        public virtual void Export()
        {
            _contentDrawer.Draw();
            _contentDrawer.Save();
        }
    }
}
