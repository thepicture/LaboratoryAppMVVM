using System;
using System.Runtime.Serialization;

namespace LaboratoryAppMVVM.Models.Exceptions
{
    public class SessionIsAlreadyEnabledException : Exception
    {
        public SessionIsAlreadyEnabledException()
        {
        }

        public SessionIsAlreadyEnabledException(string message) : base(message)
        {
        }

        public SessionIsAlreadyEnabledException(
            string message,
            Exception innerException) : base(message, innerException)
        {
        }

        protected SessionIsAlreadyEnabledException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
