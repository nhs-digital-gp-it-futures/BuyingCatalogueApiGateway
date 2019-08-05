using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Queueing
{
    public interface IRabbitMQConnectionFactory
    {
        IConnection CreateConnection();
        string GetExchangeName();
    }
}
