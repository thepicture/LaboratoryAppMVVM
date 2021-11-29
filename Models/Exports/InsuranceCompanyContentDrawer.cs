using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.Exceptions;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LaboratoryAppMVVM.Models.Exports
{
    public class InsuranceCompanyContentDrawer : ContentDrawer
    {
        private const int fontSize = 8;
        private readonly ICollection<InsuranceCompany> _insuranceCompanies;
        private readonly DateTime _from;
        private readonly DateTime _to;

        public InsuranceCompanyContentDrawer(
            IDrawingContext drawingContext,
            string saveFolderPath,
            ICollection<InsuranceCompany> insuranceCompanies,
            DateTime from, DateTime to)
            : base(drawingContext, saveFolderPath)
        {
            _insuranceCompanies = insuranceCompanies;
            _from = from;
            _to = to;
        }

        public override void Draw()
        {
            Worksheet worksheet = ((Workbook)_drawingContext.GetContext()).Sheets[1];
            int startRowIndex = 1;
            int newCompanyStartIndex = 1;
            foreach (InsuranceCompany company in _insuranceCompanies)
            {
                worksheet.Cells[1][startRowIndex] = "Название страховой компании";
                worksheet.Cells[2][startRowIndex++] = company.Name;
                worksheet.Cells[1][startRowIndex] = "Период для оплаты";
                worksheet.Cells[2][startRowIndex++] = $"С {_from} по {_to}";
                Range patientFullNameHeader = worksheet
                    .Range[worksheet.Cells[1][startRowIndex],
                           worksheet.Cells[2][startRowIndex]];
                patientFullNameHeader.Merge();
                patientFullNameHeader.Value = "ФИО пациентов "
                                              + "с оказанными им услугами";
                patientFullNameHeader.HorizontalAlignment = XlHAlign.xlHAlignCenter;
                startRowIndex++;
                foreach (Patient patient in company.Patient)
                {
                    Range patientsServicesHeader = worksheet
                        .Range[worksheet.Cells[1][startRowIndex],
                               worksheet.Cells[2][startRowIndex]];
                    patientsServicesHeader.Merge();
                    patientsServicesHeader.Value = $"Услуги пациента "
                                                   + patient.FullName;
                    patientsServicesHeader.HorizontalAlignment = XlHAlign
                        .xlHAlignCenter;
                    startRowIndex++;
                    foreach (AppliedService appliedService in patient.AppliedService)
                    {
                        worksheet.Cells[1][startRowIndex] = appliedService
                            .Service.Name;
                        worksheet.Cells[2][startRowIndex++] = appliedService.Service
                            .Price + " руб.";
                    }
                    worksheet.Cells[1][startRowIndex] = "Стоимость услуг "
                                                        + "по пациенту "
                                                        + patient.FullName;
                    worksheet.Cells[2][startRowIndex++] = patient.AppliedService
                        .Sum(s => s.Service.Price) + " руб.";
                }
                worksheet.Cells[1][startRowIndex] = "Итоговая стоимость "
                                                    + "по всем пациентам";
                worksheet.Cells[2][startRowIndex] = company.Patient
                    .Sum(p => p.AppliedService.Sum(s => s.Service.Price)) + " руб.";
                Range rangeBorders = worksheet
                    .Range
                    [
                        worksheet.Cells[1][newCompanyStartIndex],
                        worksheet.Cells[2][startRowIndex]
                    ];
                rangeBorders.Cells.Borders.LineStyle = XlLineStyle.xlContinuous;
                rangeBorders.Font.Size = fontSize;
                worksheet.Columns.AutoFit();
                startRowIndex += 2;
                newCompanyStartIndex = startRowIndex;
            }
        }

        public override void Save()
        {
            try
            {
                SaveAsCsv();
                SaveAsPdf();
            }
            catch (Exception ex)
            {
                throw new ExportException(ex.Message);
            }
            finally
            {
                _drawingContext.Dispose();
            }
        }

        private void SaveAsPdf()
        {
            string nameOfFilePdf = GetFileName()
               + ".pdf";
            string fullPathToPdf = Path.Combine(_saveFolderPath, nameOfFilePdf);
            ((_drawingContext.GetContext() as Workbook)
                .Sheets[1] as Worksheet)
                .ExportAsFixedFormat(Type: XlFixedFormatType.xlTypePDF,
                                     Filename: fullPathToPdf);
        }

        public string GetFileName()
        {
            return "ОтчётНаКаждуюСтраховуюКомпанию_"
                   + "ДатаНачала_"
                   + _from.ToString("yyyy-MM-dd_hh-mm-ss")
                   + "_"
                   + "ДатаОкончания_"
                   + _to.ToString("yyyy-MM-dd_hh-mm-ss");
        }

        private void SaveAsCsv()
        {
            string nameOfFileCsv = GetFileName()
                                + ".csv";
            string fullPathToCsv = Path.Combine(_saveFolderPath, nameOfFileCsv);
            (_drawingContext.GetContext() as Workbook)
                .Worksheets[1]
                .SaveAs(Filename: fullPathToCsv,
                        FileFormat: XlFileFormat.xlCSV);
        }
    }
}
