using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Gateway.Routing
{
    public class ExtractedRequest
    {
        public string Body { get; }
        public string Query { get; }
        public string Path { get; set; }
        public IHeaderDictionary Headers { get; }
        public string Method { get; }

        public ExtractedRequest(string path, string body, string query, IHeaderDictionary headers, string method)
        {
            Body = body;
            Query = query;
            Path = path;
            Headers = headers;
            Method = method;
        }

        public ExtractedRequest(HttpRequest request)
        {
            Query = request.QueryString.ToString();
            Path = request.Path;
            Headers = request.Headers;
            Method = request.Method;
            using(var reader = new StreamReader(request.Body))
            {
                Body = reader.ReadToEnd();
            }
        }

        public byte[] GetJsonBytes()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
