using LaboratoryAppMVVM.Models.Exceptions;
using Microsoft.Office.Interop.Word;
using System;
using System.IO;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class QualityControlChartDrawer : ContentDrawer
    {
        private readonly MemoryStream _imageBuffer;

        public QualityControlChartDrawer(IDrawingContext drawingContext,
                                                string saveFolderPath,
                                                MemoryStream imageBuffer)
            : base(drawingContext, saveFolderPath)
        {
            _imageBuffer = imageBuffer;
        }

        public override void Draw()
        {
            try
            {
                File.WriteAllBytes(GetTempFileName(), _imageBuffer.ToArray());
                Document document = _drawingContext.GetContext() as Document;
                Paragraph paragraph = document.Paragraphs.Add();
                Range range = paragraph.Range;
                _ = range.InlineShapes.AddPicture(GetTempFileName());
                range.ParagraphFormat.Alignment = WdParagraphAlignment
                    .wdAlignParagraphCenter;
            }
            catch (Exception ex)
            {
                throw new PdfExportException(ex.Message);
            }
            finally
            {
                if (File.Exists(GetTempFileName()))
                {
                    File.Delete(GetTempFileName());
                }
            }
        }

        private static string GetTempFileName()
        {
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "tempImage.png");
        }

        public override void Save()
        {
            try
            {
                string nameOfFile = "График_"
                    + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss")
                    + ".pdf";
                string fullPathToPdf = Path.Combine(_saveFolderPath, nameOfFile);
                (_drawingContext.GetContext() as Document)
                    .SaveAs(fullPathToPdf, WdSaveFormat.wdFormatPDF);
                _ = System.Diagnostics.Process.Start
                    (
                        fileName: fullPathToPdf
                    );
            }
            catch (Exception ex)
            {
                throw new PdfExportException(ex.Message);
            }
            finally
            {
                _drawingContext.Dispose();
            }
        }
    }
}
