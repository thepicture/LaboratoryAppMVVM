﻿using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using System;
using System.IO;
using System.Linq;
using Word = Microsoft.Office.Interop.Word;

namespace LaboratoryAppMVVM.Models
{
    public class OrderPdfExporter : IPdfExportable
    {
        private readonly object _lock = new object();
        private readonly Order _order;
        private readonly string _nameOfFile;

        public OrderPdfExporter(Order order, string nameOfFile)
        {
            _order = order;
            _nameOfFile = nameOfFile;
        }

        public void Export(bool isShowAfterSave, string outputPath)
        {
            lock (_lock)
            {
                Word.Application application = null;
                Word.Document document = null;
                try
                {
                    application = new Word.Application();
                    document = application.Documents.Add();
                    Word.Paragraph paragraph = document.Paragraphs.Add();
                    Word.Range range = paragraph.Range;
                    Word.Table table = range.Tables.Add(range, 8, 2);
                    table.Borders.InsideLineStyle = table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
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
                    table.Cell(7, 2).Range.Text = string.Join(", ", _order.Service.ToList().Select(s => s.Name));
                    table.Cell(8, 1).Range.Text = "Стоимость";
                    table.Cell(8, 2).Range.Text = _order.Service.Sum(s => s.Price).ToString("N2");
                    document.SaveAs(Path.Combine(outputPath, _nameOfFile), FileFormat: Word.WdSaveFormat.wdFormatPDF);

                    if (isShowAfterSave)
                    {
                        _ = System.Diagnostics.Process.Start(Path.Combine(outputPath, _nameOfFile));
                    }
                }
                catch (Exception ex)
                {
                    throw new PdfExportException(ex.Message);
                }
                finally
                {
                    document.Close(SaveChanges: Word.WdSaveOptions.wdDoNotSaveChanges);
                    application.Quit(SaveChanges: Word.WdSaveOptions.wdDoNotSaveChanges);
                }
            }
        }
    }
}
