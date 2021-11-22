using System;
using System.Runtime.Serialization;

namespace LaboratoryAppMVVM.Models.Exceptions
{
    public class PdfExportException : ExportException
    {
        public PdfExportException()
        {
        }

        public PdfExportException(string message) : base(message)
        {
        }

        public PdfExportException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PdfExportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
