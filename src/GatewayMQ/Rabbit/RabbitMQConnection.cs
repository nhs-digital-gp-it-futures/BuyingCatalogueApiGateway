using Gateway.MQ.Common;
using RabbitMQ.Client;

namespace Gateway.MQ.Rabbit
{
    public class RabbitMQConnection : IRabbitMQConnectionFactory
    {        
        private readonly ConnectionDetails connection;
        
        public RabbitMQConnection(ConnectionDetails connection)
        {
            this.connection = connection;
        }

        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = connection.HostName,                
            };
            
            return factory.CreateConnection(); ;
        }

        public string GetExchangeName()
        {
            return connection.ExchangeName;
        }
    }
}
