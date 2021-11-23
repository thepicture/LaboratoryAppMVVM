using System;

namespace LaboratoryAppMVVM.Models
{
    public class DateTimeValidator : IValidator
    {
        public bool IsValidated(params object[] values)
        {
            if (values == null || values.Length == 0)
            {
                return false;
            }
            if (values.Length != 2)
            {
                return false;
            }
            DateTime fromDate = (DateTime)values[0];
            DateTime toDate = (DateTime)values[1];

            return fromDate != null
                && toDate != null
                && fromDate <= toDate;
        }
    }
}
