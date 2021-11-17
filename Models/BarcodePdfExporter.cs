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
                Word.Application application = null;
                Word.Document document = null;
                Word.Paragraph paragraph = null;
                Word.Range range = null;
                try
                {
                    string path = outputPath ?? AppDomain.CurrentDomain.BaseDirectory;

                    application = new Word.Application();
                    document = application.Documents.Add();
                    paragraph = document.Paragraphs.Add();
                    range = paragraph.Range;
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
                finally
                {
                    if (application != null)
                    {
                        document.Close(SaveChanges: Word.WdSaveOptions.wdDoNotSaveChanges);
                        application.Quit(SaveChanges: Word.WdSaveOptions.wdDoNotSaveChanges);
                    }
                }
            }
        }
    }
}
