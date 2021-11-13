using LaboratoryAppMVVM.Models.Exceptions;
using System;
using System.IO;
using Word = Microsoft.Office.Interop.Word;

namespace LaboratoryAppMVVM.Models
{
    public class BarcodePdfExporter : IPdfExportable
    {
        private readonly object _lock = new object();
        public void Export(bool isShowAfterSave, string outputPath = null)
        {
            lock (_lock)
            {
                try
                {
                    string path = outputPath ?? AppDomain.CurrentDomain.BaseDirectory;

                    Word.Application application = new Word.Application();
                    Word.Document document = application.Documents.Add();
                    Word.Paragraph paragraph = document.Paragraphs.Add();
                    Word.Range range = paragraph.Range;
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tempBarcode.png"))
                    {
                        throw new PdfExportException("Barcode was not found");
                    }
                    _ = range.InlineShapes.AddPicture(AppDomain.CurrentDomain.BaseDirectory
                        + "tempBarcode.png");
                    range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    document.SaveAs(path + "\\barcode.pdf", FileFormat: Word.WdSaveFormat.wdFormatPDF);

                    if (isShowAfterSave)
                    {
                        _ = System.Diagnostics.Process.Start(path + "\\barcode.pdf");
                    }
                }
                catch (Exception ex)
                {
                    throw new PdfExportException(ex.Message);
                }
            }
        }
    }
}
