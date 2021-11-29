using LaboratoryAppMVVM.Models.HttpClasses;
using System.Net;
using System.Text;

namespace LaboratoryAppMVVM.Services
{
    public class JsonServicePoster : IPostable
    {
        private readonly string _url;
        private readonly string _jsonData;
        private readonly int _timeoutInSeconds;

        public JsonServicePoster(
            string url,
            string jsonData,
            int timeoutInSeconds = 30)
        {
            _url = url;
            _jsonData = jsonData;
            _timeoutInSeconds = timeoutInSeconds;
        }


        public byte[] Post()
        {
            WebClient client = new CustomTimeoutWebClient(_timeoutInSeconds);
            client.Headers.Add("Content-Type", "application/json");
            string jsonData = _jsonData;
            return client.UploadData(_url,
                              "POST",
                              Encoding.UTF8.GetBytes(jsonData));
        }
    }
}
