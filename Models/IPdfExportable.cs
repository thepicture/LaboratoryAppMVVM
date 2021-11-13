namespace LaboratoryAppMVVM.Models
{
    /// <summary>
    /// Describes classes which can export a .pdf file.
    /// </summary>
    public interface IPdfExportable
    {
        /// <summary>
        /// Saves the PDF to the given output path in the system asynchronically.
        /// </summary>
        /// <param name="outputPath">The output path of the .pdf file.</param>
        /// <param name="isShowAfterSave">Determines if the .pdf file will be opened after the save.</param>
        void Export(bool isShowAfterSave, string outputPath);
    }
}
