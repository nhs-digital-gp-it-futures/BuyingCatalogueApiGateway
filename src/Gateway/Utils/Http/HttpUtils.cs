using RestSharp;
using System;
using System.Net.Http;

namespace Gateway.Utils.Http
{
    public static class HttpUtils
    {
        public static Method GetHttpMethod(string method)
        {
            switch (method)
            {
                case "GET": return Method.GET;
                case "POST": return Method.POST;
                case "PUT": return Method.PUT;
                case "DELETE": return Method.DELETE;
                default:
                    throw new ArgumentException($"Method not supported: {method}");
            }
        }

        public static Method GetHttpMethod(HttpMethod method)
        {
            return GetHttpMethod(method.ToString());
        }
    }
}