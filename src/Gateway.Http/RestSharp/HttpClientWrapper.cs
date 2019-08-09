using Gateway.Http.PublicInterfaces;
using Gateway.Models.Exceptions;
using Gateway.Models.Requests;
using Gateway.Models.Responses;
using Gateway.Models.RouteInfo;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Gateway.Http
{
    public sealed class HttpClientWrapper : IHttpClient
    {
        private readonly Dictionary<string, string> connectionStrings;
        private readonly IRestClient restClient;

        public HttpClientWrapper(Dictionary<string, string> connectionStrings)
        {
            this.connectionStrings = connectionStrings;
            restClient = new RestClient();
        }

        public async Task<ExtractedResponse> SendRequest(ExtractedRequest request, Route route)
        {            
            var apiUrl = connectionStrings[route.Destination.ApiName] ?? throw new ArgumentNullException();

            var originalPath = request.Path;

            request.Path = ConvertEndpoint(originalPath, route);

            IRestRequest restRequest = HttpRequestMessageFactory.GenerateRequestMessage(request, apiUrl);

            var response = await restClient.ExecuteTaskAsync(restRequest);

            if(response.ErrorException != null)
            {
                throw new CustomException("Request timed out", $"Request to {originalPath} timed out", (int)System.Net.HttpStatusCode.RequestTimeout);
            }

            return new ExtractedResponse(response);
        }

        private string ConvertEndpoint(string originalPath, Route route)
        {
            return originalPath.Replace(route.Endpoint, route.Destination.Uri);
        }
    }
}
