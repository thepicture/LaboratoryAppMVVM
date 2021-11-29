namespace LaboratoryAppMVVM.Models.Exports
{
    /// <summary>
    /// Implements methods to draw 
    /// the content with the ability to save it.
    /// <typeparamref name="T"/> is a type 
    /// which will be 
    /// used for drawing.
    /// This class cannot be instantiated.
    /// </summary>
    public abstract class ContentDrawer : ICanSaveResource
    {
        protected readonly IDrawingContext _drawingContext;
        protected readonly string _saveFolderPath;

        protected ContentDrawer(
            IDrawingContext drawingContext,
            string saveFolderPath)
        {
            _drawingContext = drawingContext;
            _saveFolderPath = saveFolderPath;
        }

        /// <summary>
        /// Draws the content on the given drawing context.
        /// </summary>
        public abstract void Draw();
        /// <summary>
        /// Saves the content on the given drawing context.
        /// </summary>
        public abstract void Save();
    }
}