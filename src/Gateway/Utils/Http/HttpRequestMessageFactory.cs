using Gateway.Routing;
using RestSharp;

namespace Gateway.Utils.Http
{
    public static class HttpRequestMessageFactory
    {
        public static RestRequest GenerateRequestMessage(ExtractedRequest request, string uri)
        {
            var fullUrl = uri + request.Path;

            var restRequest = new RestRequest(fullUrl)
            {
                Method = HttpUtils.GetHttpMethod(request.Method)
            };

            if (restRequest.Method == Method.POST || restRequest.Method == Method.PUT)
            {
                restRequest.AddParameter("application/json", request.Body, ParameterType.RequestBody);
            }

            if (!string.IsNullOrEmpty(request.Query))
            {
                string[] splitQuery = request.Query.Replace("?", "").Split('&');

                foreach (var query in splitQuery)
                {
                    var queryParam = query.Split('=');
                    restRequest.AddQueryParameter(queryParam[0], queryParam[1]);
                }
            }

            foreach (var header in request.Headers)
            {
                restRequest.AddHeader(header.Key, header.Value);
            }

            return restRequest;
        }
    }
}
