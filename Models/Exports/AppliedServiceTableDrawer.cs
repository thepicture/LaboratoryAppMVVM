using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Linq;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class AppliedServiceTableDrawer : ContentDrawer
    {
        private readonly AppliedServiceReport _report;

        public AppliedServiceTableDrawer(
            IDrawingContext drawingContext,
            string saveFolderPath,
            AppliedServiceReport report) : base(drawingContext, saveFolderPath)
        {
            _report = report;
        }

        public override void Draw()
        {
            Document document = _drawingContext.GetContext() as Document;
            Paragraph paragraph = document.Paragraphs.Add();
            Range range = paragraph.Range;
            Table table = range.Tables.Add(range, 1, 2);
            table.Borders.InsideLineStyle =
                table.Borders.OutsideLineStyle =
                WdLineStyle.wdLineStyleSingle;
            table.Cell(table.Rows.Last.Index, 1).Range.Text = "Количество " +
                "оказанных услуг за период времени";
            table.Cell(table.Rows.Last.Index, 2).Range.Text = _report
                .GetAppliedServicesCount().ToString();
            _ = table.Rows.Add();
            table.Cell(table.Rows.Last.Index, 1).Range.Text = "Перечень " +
                "услуг за период времени";
            table.Cell(table.Rows.Last.Index, 2).Range.Text = string
                .Join(", ", _report.GetSetOfServicesPerPeriod()
                .Select(s => s.Name));
            _ = table.Rows.Add();
            table.Cell(table.Rows.Last.Index, 1).Range.Text = "Количество пациентов";
            table.Cell(table.Rows.Last.Index, 2).Range.Text = string.Join(
                ", ",
                _report.GetPatientsCount().ToString());
            _ = table.Rows.Add();
            table.Cell(table.Rows.Last.Index, 1).Merge(table.Cell(
                table.Rows.Last.Index,
                2));
            table.Cell(table.Rows.Last.Index, 1).Range.Text = "Количество " +
                "пациентов в день по каждой услуге";
            _ = table.Rows.Add();
            table.Rows.Last.Cells.Split(1, 2);
            foreach (Tuple<Service, int> service
                in _report.GetPatientsPerDayOfServices())
            {
                table.Cell(table.Rows.Last.Index, 1).Range.Text = service.Item1.Name;
                table.Cell(table.Rows.Last.Index, 2).Range.Text = service.Item2
                    .ToString();
                _ = table.Rows.Add();
            }
            table.Cell(table.Rows.Last.Index, 1).Merge(table.Cell(
                table.Rows.Last.Index,
                2));
            table.Cell(table.Rows.Last.Index, 1).Range.Text = $"Средний результат " +
                $"каждого исследования в день с " +
                $"{_report.FromPeriod:yyyy-MM-dd} " +
                $"по {_report.ToPeriod:yyyy-MM-dd}";
            foreach (Tuple<Service, double> service
                in _report.GetMeanResultOfServicesPerPeriod())
            {
                _ = table.Rows.Add();
                if (table.Rows.Last.Cells.Count < 2)
                {
                    table.Rows.Last.Cells.Split(1, 2);
                }
                table.Cell(table.Rows.Last.Index, 1).Range.Text = service.Item1.Name;
                table.Cell(table.Rows.Last.Index, 2).Range.Text = service.Item2
                    .ToString();
            }
        }

        public override void Save()
        {
            try
            {
                string nameOfFile = "ТаблицаОтчётаПоОказаннымУслугам_" + DateTime
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
