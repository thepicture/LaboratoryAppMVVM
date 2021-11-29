﻿using LaboratoryAppMVVM.Models.Entities;
using LaboratoryAppMVVM.Models.LaboratoryIO;
using System;
using System.Windows.Forms.DataVisualization.Charting;

namespace LaboratoryAppMVVM.Models.Exports
{
    /// <summary>
    /// Implements methods to export a presentation 
    /// as a chart or table. 
    /// <br></br>
    /// <br></br>
    /// This class cannot be instantiated.
    /// </summary>
    public abstract class PresentationExporter : IExporter
    {
        protected readonly Report _report;
        private readonly string _exportType;
        protected string _selectedSavePath;
        private readonly IBrowserDialog _dialog;

        public string SelectedSavePath
        {
            get => _selectedSavePath; set => _selectedSavePath = value;
        }

        public Report Report => _report;

        public Chart Chart { get; }

        public PresentationExporter(
            Report report,
            Chart chart,
            string exportType,
            IBrowserDialog dialog)
        {
            _report = report;
            Chart = chart;
            _exportType = exportType;
            _dialog = dialog;
        }

        public void Export()
        {
            if (!_dialog.ShowDialog())
            {
                return;
            }
            SelectedSavePath = _dialog.GetSelectedItem() as string;
            if (Chart == null)
            {
                throw new NullReferenceException("Экспорт неуспешен, потому что "
                                     + "график недоступен. "
                                     + "Пожалуйста, перезайдите на страницу"
                                     + "и попробуйте ещё раз");
            }
            switch (_exportType)
            {
                case "только график":
                    ExportChartToPdf();
                    break;
                case "только таблица":
                    ExportTableToPdf();
                    break;
                case "график и таблица":
                    ExportChartToPdf();
                    ExportTableToPdf();
                    break;
                default:
                    break;
            }
        }

        public abstract void ExportTableToPdf();

        public abstract void ExportChartToPdf();
    }
}
