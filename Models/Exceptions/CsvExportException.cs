using System;
using System.Runtime.Serialization;

namespace LaboratoryAppMVVM.Models.Exceptions
{
    public class CsvExportException : ExportException
    {
        public CsvExportException()
        {
        }

        public CsvExportException(string message) : base(message)
        {
        }

        public CsvExportException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CsvExportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
