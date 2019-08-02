using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Queueing
{
    public class RabbitMQConnection : IRabbitMQConnectionFactory
    {
        private readonly string connectionString;

        public RabbitMQConnection(string connection)
        {
            this.connectionString = connection;
        }

        public IConnection CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = connectionString
            };
            var connection = factory.CreateConnection();
            return connection;
        }
    }
}
