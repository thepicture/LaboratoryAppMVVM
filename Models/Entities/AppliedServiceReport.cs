using System;
using System.Collections.Generic;
using System.Linq;

namespace LaboratoryAppMVVM.Models.Entities
{
    public class AppliedServiceReport : Report
    {
        private readonly LaboratoryDatabaseEntities _context;
        private readonly DateTime _fromPeriod;
        private readonly DateTime _toPeriod;
        private readonly IValidator _validator;
        public IEnumerable<Tuple<Service, double>> GetMeanResultOfServicesPerPeriod()
        {
            IEnumerable<Tuple<Service, double>> MeanResultOfServicePerPeriod = new List<Tuple<Service, double>>();
            foreach (Service service in _context.Service.ToList())
            {
                if (service.AppliedService.Count == 0)
                {
                    MeanResultOfServicePerPeriod = MeanResultOfServicePerPeriod.Append(Tuple.Create(service, .0));
                }
                else
                {
                    double meanResult = service.AppliedService.Where(s =>
                    {
                        return _validator.IsValidated(
                                            _fromPeriod,
                                            _toPeriod,
                                            s.FinishedDateTime);
                    }).Select(s => s.Result).Sum() /
                            (_toPeriod - _fromPeriod).Days;
                    MeanResultOfServicePerPeriod = MeanResultOfServicePerPeriod.Append(Tuple.Create(service, meanResult));
                }
            }
            return MeanResultOfServicePerPeriod;
        }

        public IEnumerable<Tuple<Service, int>> GetPatientsPerDayOfServices()
        {
            IEnumerable<Tuple<Service, int>> MeanPatientsPerDayOfServices =
                new List<Tuple<Service, int>>();
            foreach (Service service in _context.Service.ToList())
            {
                if (service.AppliedService.Count == 0)
                {
                    MeanPatientsPerDayOfServices = MeanPatientsPerDayOfServices.Append(Tuple.Create(service, 0));
                }
                else
                {

                    int meanResult = Convert.ToInt32(Math.Floor(GetPatientsCount() /
                            (_toPeriod - _fromPeriod).TotalDays));
                    MeanPatientsPerDayOfServices = MeanPatientsPerDayOfServices.Append(Tuple.Create(service,
                        meanResult));
                }
            }
            return MeanPatientsPerDayOfServices;
        }

        private IEnumerable<AppliedService> GetAppliedServicesInPeriod()
        {
            return _context
                            .AppliedService
                            .ToList()
                            .Where(s => _validator.IsValidated(_fromPeriod,
                                                               _toPeriod,
                                                               s.FinishedDateTime));
        }

        public int GetAppliedServicesCount()
        {
            return GetAppliedServicesInPeriod().Count();
        }

        public int GetPatientsCount()
        {
            return GetAppliedServicesInPeriod().Select(s => s.Patient).Count();
        }

        public IEnumerable<Service> GetSetOfServicesPerPeriod()
        {
            return GetAppliedServicesInPeriod().Select(s => s.Service).Distinct();
        }

        public AppliedServiceReport(LaboratoryDatabaseEntities context,
                                    DateTime fromPeriod,
                                    DateTime toPeriod)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _fromPeriod = fromPeriod;
            _toPeriod = toPeriod;
            _validator = new DateTimeIsInPeriodValidator();
        }
    }
}
