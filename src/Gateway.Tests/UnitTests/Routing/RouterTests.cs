using FluentAssertions;
using Gateway.Http.PublicInterfaces;
using Gateway.Models.Exceptions;
using Gateway.Models.Requests;
using Gateway.Models.RouteInfo;
using Gateway.MQ.Interfaces;
using Gateway.Routing;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Gateway.Tests.UnitTests.Routing
{
    public class RouterTests
    {
        [Fact]
        public void Router_GetEndpoints_TypeMQ()
        {
            var routes = Router.GetRoutes(Path.Combine(Environment.CurrentDirectory, "Routes", "MQ"), TransportType.MessageQueue);

            routes.Should().AllBeOfType<Route>();
            foreach (var route in routes)
            {
                route.TransportType.Should().Be(TransportType.MessageQueue);
            }
        }

        [Fact]
        public void Router_GetEndpoints_TypeHttp()
        {
            var routes = Router.GetRoutes(Path.Combine(Environment.CurrentDirectory, "Routes", "Http"), TransportType.Http);

            routes.Should().AllBeOfType<Route>();
            foreach (var route in routes)
            {
                route.TransportType.Should().Be(TransportType.Http);
            }
        }

        [Fact]
        public void Router_UndefinedRoute()
        {
            // Assemble
            var messageClient = new Mock<IMessageClient>();
            var httpClient = new Mock<IHttpClient>();            
            var router = new Router(messageClient.Object, httpClient.Object);
            var headers = new HeaderDictionary
            {
                { "X-Correlation-Id", Guid.NewGuid().ToString() }
            };
            ExtractedRequest request = new ExtractedRequest("/bob", "", "", headers, "GET");

            Func<Task> act = async () => await router.RouteRequest(request);
            act.Should().Throw<NotFoundCustomException>("Route not found");
        }
    }
}