namespace LaboratoryAppMVVM.Models.Exports
{
    /// <summary>
    /// Defines a method 
    /// for exporting data 
    /// from one representation to another.
    /// </summary>
    public abstract class Exporter<TContext>
    {
        protected readonly ContentDrawer<TContext> _contentDrawer;

        protected Exporter(ContentDrawer<TContext> contentDrawer)
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
