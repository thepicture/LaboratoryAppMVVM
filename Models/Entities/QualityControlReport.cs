using System;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.Models.Entities
{
    public class QualityControlReport
    {
        private readonly Service _service;

        public QualityControlReport(Service service)
        {
            _service = service;
        }

        public ICollection<AppliedService> GetServices()
        {
            if (_service.AppliedService.Count == 0)
            {
                return new List<AppliedService>();
            }
            return _service.AppliedService.ToList();
        }

        public double GetVariationCoefficient()
        {
            return GetMeanQuadrantDeviation() / GetMeanValueOfService() * 100;
        }

        public double GetMeanQuadrantDeviation()
        {
            double meanValueOfService = GetMeanValueOfService();
            double meanQuadrantDeviation = 0;
            foreach (AppliedService appliedService in _service.AppliedService)
            {
                meanQuadrantDeviation += Math.Pow(
                    meanValueOfService - appliedService.Result,
                    2);
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
