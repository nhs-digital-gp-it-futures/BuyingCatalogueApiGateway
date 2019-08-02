using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Gateway.Models.Messages
{
    public class AuditMessage
    {
        public string Endpoint { get; }
        public string Query { get; }
        public string Body { get; }
        public string Method { get; }
        public IHeaderDictionary Headers { get; }
        public int StatusCode { get; }

        public AuditMessage(string endpoint, string method, IHeaderDictionary headers, string query, string body)
        {
            Endpoint = endpoint;
            Query = query;
            Body = body;
            Method = method;
            Headers = headers;
        }

        public AuditMessage(IHeaderDictionary headers, int statusCode, string body = null)
        {
            Body = body;
            Headers = headers;
            StatusCode = statusCode;
        }

        public string GenerateAuditMessage()
        {
            return JsonConvert.SerializeObject(this,
                Formatting.None,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
                });
        }
    }
}
