using RabbitMQ.Client;

namespace Gateway.MQ.Rabbit
{
    public interface IRabbitMQConnectionFactory
    {
        IConnection CreateConnection();
        string GetExchangeName();
    }
}
