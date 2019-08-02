using Gateway.Queueing;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using RabbitMQ.Client.Events;

namespace Gateway.Tests.UnitTests.Queueing
{
    public class SendMessageTests
    {
        [Fact]
        public void Send_FireAndForget()
        {
            var connFactory = new ConnectionFactory();
            var conn = connFactory.CreateConnection();
            var channel = conn.CreateModel();

            var sendMessage = new SendMessage(channel);
            var guid = Guid.NewGuid();

            sendMessage.SendCall("thisIsTheMessage", "test_queue", guid);
        }

        [Fact]
        public async void Send_GetResponse()
        {
            var connFactory = new ConnectionFactory();
            var conn = connFactory.CreateConnection();
            var channel = conn.CreateModel();

            var sendMessage = new SendMessage(channel);
            var guid = Guid.NewGuid();

            channel.QueueDeclare("test_queue", false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArgs) =>
            {
                channel.BasicPublish(exchange: "", routingKey: eventArgs.BasicProperties.ReplyTo, basicProperties: eventArgs.BasicProperties, body: eventArgs.Body);
            };

            var message = sendMessage.SendAndReceiveCall("thisIsTheMessage", "test_queue", guid).ContinueWith(s => s.Result.Should().Be(""));
        }
    }
}
