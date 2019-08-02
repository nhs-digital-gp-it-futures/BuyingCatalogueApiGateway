using Gateway.Utils.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Models
{
    public class ExtractedResponse
    {
        public string Body { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public IHeaderDictionary Headers { get; set; }

        public ExtractedResponse(Microsoft.AspNetCore.Http.HttpResponse response)
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

        public byte[] GetJsonBytes()
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
