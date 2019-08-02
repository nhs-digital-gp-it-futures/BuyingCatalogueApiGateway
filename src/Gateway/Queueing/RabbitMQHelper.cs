using Gateway.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Queueing
{
    public class RabbitMQHelper
    {
        private static IModel _model;

        public RabbitMQHelper(IModel model)
        {
            _model = model;
        }

        public void SetupQueue(string queueName)
        {
            _model.ExchangeDeclare("GPITFBC", "direct");
            _model.QueueDeclare(queueName, false, false, false, null);
            _model.QueueBind(queueName, "GPITFBC", queueName);
        }
    }
}
