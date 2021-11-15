using System;
using System.Windows.Forms;

namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Exports the .pdf file with the selected output path.
    /// </summary>
    public class CustomPathPdfExporter
    {
        private readonly IPdfExportable _pdfExportable;

        public CustomPathPdfExporter(IPdfExportable pdfExportable)
        {
            _pdfExportable = pdfExportable;
        }

        /// <summary>
        /// Proxy for IPdfExportable's Export method.
        /// User can choose path by the dialogue window.
        /// </summary>
        /// <param name="isShowAfterSave">Determines if the .pdf file will be opened after saving.</param>
        /// <returns></returns>
        public string Save(bool isShowAfterSave = true)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _pdfExportable.Export(isShowAfterSave, folderBrowserDialog.SelectedPath);
                return folderBrowserDialog.SelectedPath;
            }
            else
            {
                _pdfExportable.Export(isShowAfterSave, AppDomain.CurrentDomain.BaseDirectory);
            }
            return AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}