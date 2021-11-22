using Microsoft.Office.Interop.Excel;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class ExcelDrawingContext : IDrawingContext
    {
        private readonly Application _application;
        private Workbook _workbook;
        private bool _disposed = false;

        public ExcelDrawingContext()
        {
            _application = new Application
            {
                Visible = false
            };
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
                _workbook?.Close(SaveChanges: XlSaveAction.xlDoNotSaveChanges);
                _application?.Quit();
            }
            _disposed = true;
        }

        public object GetContext()
        {
            if (_workbook == null)
            {
                _ = _application.Workbooks.Add();
                _workbook = _application.ActiveWorkbook;
            }
            return _workbook;
        }
    }
}
