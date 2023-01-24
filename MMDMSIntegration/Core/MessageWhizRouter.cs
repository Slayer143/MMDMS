using System.IO;
using System.Net;

namespace Terrasoft.Configuration.MMDMS.Core
{
    public class MessageWhizRouter
    {
        private string _result { get; set; }

        private string _route { get; set; }

        private string _messageWhizAuthToken { get; set; }

        public MessageWhizRouter(string messageWhizAuthToken, string route = "")
        {
            _route = route;
            _messageWhizAuthToken = messageWhizAuthToken;
        }

        public void ChangeRoute(string route) => _route = route;

        public string GetRequestResult(string requestType, string body = "")
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(_route);

            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = requestType;
            httpWebRequest.Headers.Add("apikey", _messageWhizAuthToken);

            if (body.Length > 0)
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(body);
                }
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                _result = streamReader.ReadToEnd();
            }

            return _result;
        }
    }
}