using System.Net;

namespace LaboratoryAppMVVM.Models.HttpClasses
{
    public class JsonResearchStatusGetter : IGettable
    {
        private readonly string _url;
        private readonly int _timeoutInSeconds;

        public JsonResearchStatusGetter(string url, int timeoutInSeconds = 30)
        {
            _url = url;
            _timeoutInSeconds = timeoutInSeconds;
        }

        public byte[] Get()
        {
            WebClient client = new CustomTimeoutWebClient(_timeoutInSeconds);
            return client.DownloadData(_url);
        }
    }
}
