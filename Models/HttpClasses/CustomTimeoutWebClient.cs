using System;
using System.Net;

namespace LaboratoryAppMVVM.Models.HttpClasses
{
    /// <summary>
    /// Implements a method 
    /// to post a web request 
    /// with the given timeout 
    /// before an interruption.
    /// </summary>
    public class CustomTimeoutWebClient : WebClient
    {
        private readonly int _responseWaitTimeoutInSeconds;

        /// <summary>
        /// Initializes a new instance of the System.Net.WebClient class 
        /// with the given response wait timeout in seconds.
        /// </summary>
        /// <param name="responseWaitTimeoutInSeconds">The response wait 
        /// timeout in seconds.</param>
        public CustomTimeoutWebClient(int responseWaitTimeoutInSeconds)
        {
            if (responseWaitTimeoutInSeconds < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(responseWaitTimeoutInSeconds),
                    "Timeout must be a positive integer or zero");
            }
            _responseWaitTimeoutInSeconds = responseWaitTimeoutInSeconds;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            webRequest.Timeout = Convert.ToInt32
                (
                    TimeSpan
                    .FromSeconds(_responseWaitTimeoutInSeconds)
                    .TotalMilliseconds
                );
            return webRequest;
        }
    }
}
