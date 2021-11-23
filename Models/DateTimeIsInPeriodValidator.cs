using System;

namespace LaboratoryAppMVVM.Models
{
    public class DateTimeIsInPeriodValidator : IValidator
    {
        public bool IsValidated(params object[] values)
        {
            if (values == null || values.Length == 0)
            {
                return false;
            }
            if (values.Length != 3)
            {
                return false;
            }
            DateTime fromDate = (DateTime)values[0];
            DateTime toDate = (DateTime)values[1];
            DateTime betweenDate = (DateTime)values[2];

            return betweenDate <= toDate
                        && betweenDate >= fromDate;
        }
    }
}
