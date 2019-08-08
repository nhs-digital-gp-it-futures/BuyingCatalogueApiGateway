using Gateway.MQ.Common;
using Gateway.MQ.Interfaces;
using RabbitMQ.Client;

namespace Gateway.MQ.Rabbit
{
    public sealed class RabbitMQHelper : IMessageClient
    {
        private static IModel _model;
        private static string _exchangeName;        

        public RabbitMQHelper(ConnectionDetails details)
        {
            IRabbitMQConnectionFactory factory = new RabbitMQConnection(details);
            IConnection connection = factory.CreateConnection();
            _model = connection.CreateModel();
            _exchangeName = details.ExchangeName;
        }

        public RabbitMQHelper(IConnection connection, string exchange)
        {
            _model = connection.CreateModel();
            _exchangeName = exchange;
        }

        public void SetupQueue(string queueName)
        {
            _model.ExchangeDeclare(_exchangeName, "direct");
            _model.QueueDeclare(queueName, false, false, false, null);
            _model.QueueBind(queueName, _exchangeName, queueName);
        }

        public void PushMessageIntoQueue(byte[] message, string queue)
        {
            SetupQueue(queue);
            _model.BasicPublish(_exchangeName, queue, null, message);
        }

        public byte[] ReadMessageFromQueue(string queueName)
        {
            SetupQueue(queueName);
            byte[] message;
            var data = _model.BasicGet(queueName, false);
            message = data.Body;
            _model.BasicAck(data.DeliveryTag, false);
            return message;
        }

        public void PushAuditMessage(byte[] message)
        {
            PushMessageIntoQueue(message, "audit");
        }
    }
}
