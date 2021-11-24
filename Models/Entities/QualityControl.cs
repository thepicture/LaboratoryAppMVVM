using System;
using System.Linq;

namespace LaboratoryAppMVVM.Models.Entities
{
    public class QualityControl
    {
        private readonly Service _service;

        public QualityControl(Service service)
        {
            _service = service;
        }

        public double GetMeanQuadrantDeviation()
        {
            double meanValueOfService = GetMeanValueOfService();
            double meanQuadrantDeviation = 0;
            foreach (AppliedService appliedService in _service.AppliedService)
            {
                meanQuadrantDeviation += Math.Pow(meanValueOfService - appliedService.Result, 2);
            }
            meanQuadrantDeviation /= _service.AppliedService.Count;
            return meanQuadrantDeviation;
        }

        public double GetMeanValueOfService()
        {
            return _service.AppliedService.Sum(s => s.Result) / _service.AppliedService.Count;
        }

        public double GetStatisticLimit(int number)
        {
            return GetMeanValueOfService() + (GetMeanQuadrantDeviation() * number);
        }
    }
}
