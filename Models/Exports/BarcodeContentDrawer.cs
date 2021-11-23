using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using Microsoft.Office.Interop.Word;
using System;
using System.IO;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class BarcodeContentDrawer : ContentDrawer
    {
        private readonly Barcode _barcode;

        public BarcodeContentDrawer(IDrawingContext drawingContext,
                                    string saveFolderPath,
                                    Barcode barcode) : base(drawingContext, saveFolderPath)
        {
            _barcode = barcode;
        }

        public override void Draw()
        {
            Document document = _drawingContext.GetContext() as Document;
            Paragraph paragraph = document.Paragraphs.Add();
            Range range = paragraph.Range;
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "tempBarcode.png"))
            {
                throw new PdfExportException("Barcode was not found");
            }
            _ = range.InlineShapes.AddPicture(_barcode.ImagePath);
            range.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
        }

        public override void Save()
        {
            try
            {
                string nameOfFile = "Баркод" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".pdf";
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
