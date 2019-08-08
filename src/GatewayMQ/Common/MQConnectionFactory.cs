using Gateway.MQ.Interfaces;
using Gateway.MQ.Rabbit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gateway.MQ.Common
{
    enum Providers
    {
        RabbitMQ
    }

    public static class MQConnectionFactory
    {
        public static IMessageClient GetMessageClient(ConnectionDetails details)
        {
            var provider = Enum.Parse(typeof(Providers), details.MQProvider);
            IMessageClient client;

            switch (provider)
            {
                case Providers.RabbitMQ:
                    client = new RabbitMQHelper(details);
                    break;
                default:
                    throw new NotSupportedException($"{provider} is not supported at this time");
            }

            return client;
        }
    }
}
