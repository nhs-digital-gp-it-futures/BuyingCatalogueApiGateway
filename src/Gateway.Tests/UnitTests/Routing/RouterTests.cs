using FluentAssertions;
using Gateway.MQ.Interfaces;
using Gateway.Routing;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.IO;
using System.Net;
using Xunit;

namespace Gateway.Tests.UnitTests.Routing
{
    public class RouterTests
    {
        [Fact]
        public void Router_GetEndpoints_TypeMQ()
        {
            var routes = Router.GetRoutes(Path.Combine(Environment.CurrentDirectory, "Routes", "MQ"), Gateway.Routing.TransportType.MessageQueue);

            routes.Should().AllBeOfType<Route>();
            foreach(var route in routes)
            {
                route.TransportType.Should().Be(Gateway.Routing.TransportType.MessageQueue);
            }
        }

        [Fact]
        public void Router_GetEndpoints_TypeHttp()
        {
            var routes = Router.GetRoutes(Path.Combine(Environment.CurrentDirectory, "Routes", "Http"), Gateway.Routing.TransportType.Http);

            routes.Should().AllBeOfType<Route>();
            foreach (var route in routes)
            {
                route.TransportType.Should().Be(Gateway.Routing.TransportType.Http);
            }
        }

        [Fact]
        public async void Router_UndefinedRoute()
        {
            // Assemble
            var messageClient = new Mock<IMessageClient>();
            var router = new Router(messageClient.Object);
            var headers = new HeaderDictionary
            {
                { "X-Correlation-Id", Guid.NewGuid().ToString() }
            };
            ExtractedRequest request = new ExtractedRequest("/bob", "", "", headers, "GET");

            //Action
            var response = await router.RouteRequest(request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Body.Should().Be("Unable to locate path");
            response.Headers.Should().ContainKey("X-Correlation-Id").And.ContainValue(headers["X-Correlation-Id"]);
        }
    }
}
