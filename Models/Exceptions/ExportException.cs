using System;
using System.Runtime.Serialization;

namespace LaboratoryAppMVVM.Models.Exceptions
{
    public class ExportException : Exception
    {
        public ExportException()
        {
        }

        public ExportException(string message) : base(message)
        {
        }

        public ExportException(
            string message,
            Exception innerException) : base(message, innerException)
        {
        }

        protected ExportException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
