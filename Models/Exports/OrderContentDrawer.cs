using LaboratoryAppMVVM.Models.Entities;
using Word = Microsoft.Office.Interop.Word;
using System;
using System.Linq;
using Microsoft.Office.Interop.Word;
using LaboratoryAppMVVM.Models.Exceptions;
using System.IO;

namespace LaboratoryAppMVVM.Models.Exports
{
    class OrderContentDrawer : ContentDrawer<Order>
    {
        private readonly Order _order;

        public OrderContentDrawer(IDrawingContext drawingContext,
                                  string saveFolderPath,
                                  Order order) : base(drawingContext, saveFolderPath)
        {
            _order = order;
        }

        public override void Draw()
        {
            Document document = _drawingContext.GetContext() as Document;
            Paragraph paragraph = document.Paragraphs.Add();
            Range range = paragraph.Range;
            Table table = range.Tables.Add(range, 8, 2);
            table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = WdLineStyle.wdLineStyleSingle;
            table.Cell(1, 1).Range.Text = "Дата заказа";
            table.Cell(1, 2).Range.Text = _order.Date.ToString("yyyy-MM-ddThh:mm:ss");
            table.Cell(2, 1).Range.Text = "Номер заказа";
            table.Cell(2, 2).Range.Text = _order.Id.ToString();
            table.Cell(3, 1).Range.Text = "Номер пробирки";
            table.Cell(3, 2).Range.Text = _order.BarcodeOfPatient.Barcode;
            table.Cell(4, 1).Range.Text = "Номер страхового полиса";
            table.Cell(4, 2).Range.Text = _order.Patient.InsurancePolicyNumber ?? "Не указан";
            table.Cell(5, 1).Range.Text = "ФИО";
            table.Cell(5, 2).Range.Text = _order.Patient.FullName;
            table.Cell(6, 1).Range.Text = "Дата рождения";
            table.Cell(6, 2).Range.Text = _order.Patient.BirthDate.ToString("yyyy-MM-dd");
            table.Cell(7, 1).Range.Text = "Перечень услуг";
            table.Cell(7, 2).Range.Text = string.Join(", ", _order.AppliedService.ToList().Select(s => s.Service.Name));
            table.Cell(8, 1).Range.Text = "Стоимость";
            table.Cell(8, 2).Range.Text = _order.AppliedService.Sum(s => s.Service.Price).ToString("N2");
        }

        public override void Save()
        {
            try
            {
                string nameOfFile = "Заказ" + DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss") + ".pdf";
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
