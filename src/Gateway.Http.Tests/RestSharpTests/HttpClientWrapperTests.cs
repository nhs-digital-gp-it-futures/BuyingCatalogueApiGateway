using FluentAssertions;
using Gateway.Models.Requests;
using Gateway.Models.RouteInfo;
using Microsoft.AspNetCore.Http;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Xunit;

namespace Gateway.Http.Tests.RestSharpTests
{
    public class HttpClientWrapperTests
    {
        [Fact]
        public async void HttpClientWrapper_ReturnsResponse()
        {
            var expectedResponse = new RestResponse()
            {
                StatusCode = HttpStatusCode.OK,
                Content = "bob"
            };

            var mockedRestClient = new Mock<IRestClient>();

            mockedRestClient.Setup(s => s.ExecuteTaskAsync(It.IsAny<IRestRequest>())).ReturnsAsync(expectedResponse);

            var routes = new Dictionary<string, string>()
            {
                {"bob", "https://localhost:12345" }
            };

            var request = new ExtractedRequest("/bob", "bob", "", new HeaderDictionary(), "GET");

            var route = new Route()
            {
                Endpoint = "/bob",
                Destination = new Destination()
                {
                    ApiName = "bob",
                    Uri = "/bob"
                }
            };

            var httpClient = new HttpClientWrapper(routes, mockedRestClient.Object);            

            var response = await httpClient.SendRequest(request, route);

            response.Body.Should().Be(expectedResponse.Content);
            response.StatusCode.Should().Be(expectedResponse.StatusCode);
        }
    }
}
