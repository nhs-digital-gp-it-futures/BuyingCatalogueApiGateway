using FluentAssertions;
using Gateway.Routing;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Gateway.Tests.UnitTests.Utils
{
    public class MQClientWrapperTests
    {        
        [Fact]
        public void MQClientWrapper_CallSuccess()
        {
            var connFactory = new ConnectionFactory();
            var conn = connFactory.CreateConnection();           

            Guid guid = Guid.NewGuid();
            using (var channel = conn.CreateModel())
            {
                var wrapper = new MQClientWrapper(channel, "test_queue", guid.ToString());

                var message = Encoding.UTF8.GetBytes("bob");

                wrapper.Call(message);
            }
        }

        [Fact]
        public void MQClientWrapper_ConsumeSuccess()
        {
            var connFactory = new ConnectionFactory();
            var conn = connFactory.CreateConnection();

            Guid guid = Guid.NewGuid();
            using (var channel = conn.CreateModel())
            {
                var wrapper = new MQClientWrapper(channel, "test_queue", guid);                               

                var response = wrapper.Consume();
                response.Should().Contain("amq");
            }
        }
    }
}
