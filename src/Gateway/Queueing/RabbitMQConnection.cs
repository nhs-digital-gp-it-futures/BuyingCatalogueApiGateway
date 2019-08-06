using RabbitMQ.Client;

namespace Gateway.Queueing
{
    public class RabbitMQConnection : IRabbitMQConnectionFactory
    {        
        private readonly RabbitMQConnectionDetails connection;
        
        public RabbitMQConnection(RabbitMQConnectionDetails connection)
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
