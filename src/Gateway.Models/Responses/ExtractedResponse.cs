using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Net;
using System.Text;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace Gateway.Models.Responses
{
    public sealed class ExtractedResponse
    {
        public string Body { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();

        public ExtractedResponse(HttpResponse response)
        {            
            using (var reader = new StreamReader(response.Body))
            {
                Body = reader.ReadToEnd();
            }

            StatusCode = (HttpStatusCode)response.StatusCode;
        }

        public ExtractedResponse(IRestResponse response)
        {            
            Body = response.Content;
            StatusCode = response.StatusCode;
        }

        public ExtractedResponse(string content, HttpStatusCode statusCode)
        {
            Body = content;
            StatusCode = statusCode;
        }

        public ExtractedResponse()
        {
        }

        public byte[] GetJsonBytes()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
