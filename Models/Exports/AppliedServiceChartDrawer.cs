using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Linq;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class AppliedServiceChartDrawer : ContentDrawer
    {
        private readonly MemoryStream _imageBuffer;
        private readonly AppliedServiceReport _report;

        public AppliedServiceChartDrawer(IDrawingContext drawingContext,
                                                string saveFolderPath,
                                                MemoryStream imageBuffer,
                                                AppliedServiceReport report)
            : base(drawingContext, saveFolderPath)
        {
            _imageBuffer = imageBuffer;
            _report = report;
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
                range.ParagraphFormat.Alignment =
                    WdParagraphAlignment.wdAlignParagraphCenter;
                range.InsertParagraphAfter();

                range = document.Paragraphs.Last.Range;
                Table table = range.Tables.Add(range, 3, 2);
                table.Borders.InsideLineStyle =
                    table.Borders.OutsideLineStyle =
                    WdLineStyle.wdLineStyleSingle;
                table.Range.ParagraphFormat.Alignment =
                    WdParagraphAlignment.wdAlignParagraphCenter;
                Cell cell = table.Cell(1, 1);
                cell.Range.Text = "Количество оказанных услуг за период времени";
                cell = table.Cell(1, 2);
                cell.Range.Text = _report.GetAppliedServicesCount().ToString();

                cell = table.Cell(2, 1);
                cell.Range.Text = "Перечень услуг за период времени";
                cell = table.Cell(2, 2);
                cell.Range.Text = string.Join(", ",
                    _report.GetSetOfServicesPerPeriod()
                    .ToList()
                    .Select(s => s.Name));

                cell = table.Cell(3, 1);
                cell.Range.Text = "Количество пациентов";
                cell = table.Cell(3, 2);
                cell.Range.Text = _report.GetPatientsCount().ToString();
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
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
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
