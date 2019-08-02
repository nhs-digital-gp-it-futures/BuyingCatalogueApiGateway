using Gateway.Models;
using Gateway.Utils;
using Gateway.Utils.Http;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gateway.Routing
{
    public static class HttpClientWrapper
    {
        internal static async Task<ExtractedResponse> SendRequest(IRestRequest request)
        {
            var restClient = new RestClient();

            var response = await restClient.ExecuteTaskAsync(request);

            return new ExtractedResponse(response);
        }
    }
}
