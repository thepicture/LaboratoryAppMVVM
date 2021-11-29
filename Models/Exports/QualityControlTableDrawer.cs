using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using Microsoft.Office.Interop.Word;
using System;
using System.IO;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class QualityControlTableDrawer : ContentDrawer
    {
        private readonly QualityControlReport _qualityControl;

        public QualityControlTableDrawer(IDrawingContext drawingContext,
                                         string saveFolderPath,
                                         QualityControlReport qualityControl)
            : base(drawingContext, saveFolderPath)
        {
            _qualityControl = qualityControl;
        }

        public override void Draw()
        {
            Document document = _drawingContext.GetContext() as Document;
            Paragraph paragraph = document.Paragraphs.Add();
            Range range = paragraph.Range;
            Table table = range.Tables.Add(range, _qualityControl.GetServices().Count
                                                  + 1, 2);
            table.Borders.InsideLineStyle =
                table.Borders.OutsideLineStyle =
                WdLineStyle.wdLineStyleSingle;
            int startRowIndex = 1;
            table.Cell(startRowIndex, 1).Range.Text = "Дата и время исследования";
            table.Cell(startRowIndex++, 2).Range.Text = "Предел значений";
            foreach (AppliedService service in _qualityControl.GetServices())
            {
                table.Cell(startRowIndex, 1).Range.Text = service.FinishedDateTime
                    .ToString("yyyy-MM-dd hh:mm:ss");
                table.Cell(startRowIndex++, 2).Range.Text = service.Result
                    .ToString("N2");
            }
            range.InsertParagraphAfter();
            _ = document.Paragraphs.Add();
            paragraph = document.Paragraphs.Last;
            range = paragraph.Range;

            table = range.Tables.Add(range, 2, 2);
            table.Borders.InsideLineStyle =
             table.Borders.OutsideLineStyle =
             WdLineStyle.wdLineStyleSingle;
            table.Cell(1, 1).Range.Text = "Среднее отклонение";
            table.Cell(1, 2).Range.Text = _qualityControl.GetMeanQuadrantDeviation()
                .ToString("N2");
            table.Cell(2, 1).Range.Text = "Коэффициент вариации";
            table.Cell(2, 2).Range.Text = _qualityControl.GetVariationCoefficient()
                .ToString("N2") + " %";
        }

        public override void Save()
        {
            try
            {
                string nameOfFile = "ТаблицаКонтроляКачества_" + DateTime
                    .Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".pdf";
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
