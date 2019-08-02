using FluentAssertions;
using Gateway.Queueing;
using Gateway.Routing;
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
            var mockedModel = new Mock<IModel>();
            mockedModel.Setup(s => s.QueueDeclare(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object>>()));

            var router = new Router();

            var routes = router.GetRoutes(Path.Combine(Environment.CurrentDirectory, "Routes", "MQ"), TransportType.MessageQueue);

            routes.Should().AllBeOfType<Route>();
            foreach(var route in routes)
            {
                route.TransportType.Should().Be(TransportType.MessageQueue);
            }
        }

        [Fact]
        public void Router_GetEndpoints_TypeHttp()
        {
            var router = new Router();

            var routes = router.GetRoutes(Path.Combine(Environment.CurrentDirectory, "Routes", "Http"), TransportType.Http);

            routes.Should().AllBeOfType<Route>();
            foreach (var route in routes)
            {
                route.TransportType.Should().Be(TransportType.Http);
            }
        }
    }
}
