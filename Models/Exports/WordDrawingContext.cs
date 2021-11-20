using Microsoft.Office.Interop.Word;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class WordDrawingContext : IDrawingContext
    {
        private readonly Application _application;
        private Document _document;
        private bool _disposed = false;

        public WordDrawingContext()
        {
            _application = new Application();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                _document?.Close(SaveChanges: WdSaveOptions.wdDoNotSaveChanges);
                _application?.Quit(SaveChanges: WdSaveOptions.wdDoNotSaveChanges);
            }
            _disposed = true;
        }

        public object GetContext()
        {
            if (_document == null)
            {
                _document = _application.Documents.Add();
            }
            return _document;
        }
    }
}
