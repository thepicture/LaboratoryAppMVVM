using System;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.Models.Entities
{
    public class QualityControlReport : Report
    {
        private const int toPercentCoefficient = 100;
        private const int secondPowerOfNumber = 2;
        private readonly Service _service;

        public QualityControlReport(Service service)
        {
            _service = service;
        }

        public ICollection<AppliedService> GetServices()
        {
            return _service.AppliedService.Count == 0
                ? new List<AppliedService>()
                : _service.AppliedService.ToList();
        }

        public double GetVariationCoefficient()
        {
            return GetMeanQuadrantDeviation()
                / GetMeanValueOfService()
                * toPercentCoefficient;
        }

        public double GetMeanQuadrantDeviation()
        {
            double meanValueOfService = GetMeanValueOfService();
            double meanQuadrantDeviation = default;
            foreach (AppliedService appliedService in _service.AppliedService)
            {
                meanQuadrantDeviation += Math.Pow(
                    meanValueOfService - appliedService.Result,
                    secondPowerOfNumber);
            }
            meanQuadrantDeviation /= _service.AppliedService.Count;
            return meanQuadrantDeviation;
        }

        public double GetMeanValueOfService()
        {
            return _service.AppliedService.Sum(s => s.Result)
                   / _service.AppliedService.Count;
        }

        public double GetStatisticLimit(int number)
        {
            return GetMeanValueOfService() + (GetMeanQuadrantDeviation() * number);
        }
    }
}
