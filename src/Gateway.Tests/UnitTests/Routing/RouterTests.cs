using FluentAssertions;
using Gateway.Queueing;
using Gateway.Routing;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            foreach(var route in routes)
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
    }
}
