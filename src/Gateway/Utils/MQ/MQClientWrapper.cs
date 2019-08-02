using Gateway.Queueing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace Gateway.Routing
{
    public sealed class MQClientWrapper
    {
        private readonly IModel channel;
        private readonly string queueName;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;

        public MQClientWrapper(IModel channel, string queueName, Guid correlationId)
        {
            this.queueName = queueName;

            this.channel = channel;

            replyQueueName = this.channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = this.channel.CreateBasicProperties();
            props.CorrelationId = correlationId.ToString();
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId.ToString())
                {
                    respQueue.Add(response);
                }
            };
        }

        public MQClientWrapper(IModel model, string queueName, string replyQueue)
        {
            channel = model;
            consumer = new EventingBasicConsumer(channel);
            this.queueName = queueName;
            replyQueueName = replyQueue;

            consumer.Received += (m, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                respQueue.Add(response);
            };
        }

        public string CallAndReceive(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            Call(messageBytes);

            Consume();

            return respQueue.Take();
        }

        public void Call(byte[] message)
        {
            channel.BasicPublish(
                exchange: "",
                routingKey: queueName,
                basicProperties: props,
                body: message);
        }

        public string Consume()
        {
            return channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);
        }
    }
}