using FluentAssertions;
using Gateway.Queueing;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using RabbitMQ.Fakes;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Gateway.Tests.UnitTests.Queueing
{
    public class RabbitMQHelperTests
    {
        readonly RabbitServer server;
        readonly IConnection connection;

        public RabbitMQHelperTests()
        {
            server = new RabbitServer();
            connection = new FakeConnection(server);
        }

        [Fact]
        public void RabbitMQ_CanSetupQueue()
        {
            var helper = new RabbitMQHelper(connection, "test_exchange");
            helper.SetupQueue("bob");
        }

        [Fact]
        public void RabbitMQ_PushIntoQueue()
        {
            string exchangeName = "test_exchange";

            var helper = new RabbitMQHelper(connection, exchangeName);
            var message = Encoding.UTF8.GetBytes("bob");
            helper.PushMessageIntoQueue(message, "Test123");
            server.Exchanges[exchangeName].Messages.Should().HaveCount(1);
        }

        [Fact]
        public void RabbitMQ_ReceiveMessage()
        {
            var rabbitServer = new RabbitServer();

            var connectionFactory = new FakeConnectionFactory(rabbitServer);
            using (var connection = connectionFactory.CreateConnection())
            {
                var helper = new RabbitMQHelper(connection, "test_exchange");

                var message = Encoding.UTF8.GetBytes("bob");

                helper.PushMessageIntoQueue(message, "my_queue");

                // First message
                var response = helper.ReadMessageFromQueue("my_queue");

                response.Should().NotBeNull();

                var messageBody = Encoding.UTF8.GetString(message);

                messageBody.Should().Be("bob");
            }
        }
    }
}
