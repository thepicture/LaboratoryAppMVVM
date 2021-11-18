using System;
using System.Net;

namespace LaboratoryAppMVVM.Models.Http
{
    /// <summary>
    /// Web client with the 30 seconds timeout.
    /// </summary>
    public class ThirtySecondsTimeoutWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);
            webRequest.Timeout = Convert.ToInt32(TimeSpan.FromSeconds(30).TotalMilliseconds);
            return webRequest;
        }
    }
}
