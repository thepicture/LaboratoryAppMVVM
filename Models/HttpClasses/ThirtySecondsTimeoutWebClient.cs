using System;
using System.Net;

namespace LaboratoryAppMVVM.Models.HttpClasses
{
    /// <summary>
    /// Web client with the 30 seconds timeout.
    /// </summary>
    public class ThirtySecondsTimeoutWebClient : WebClient
    {
        private const int thirtySecondsTimeout = 30;

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            webRequest.Timeout = Convert.ToInt32(TimeSpan.FromSeconds(thirtySecondsTimeout).TotalMilliseconds);
            return webRequest;
        }
    }
}
