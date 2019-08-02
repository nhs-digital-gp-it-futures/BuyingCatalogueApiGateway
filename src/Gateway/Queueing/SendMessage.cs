using Gateway.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway.Queueing
{
    public class SendMessage
    {
        private readonly IModel Channel;

        public SendMessage(IModel channel)
        {
            Channel = channel;
        }

        public async Task<string> SendAndReceiveCall(string message, string queueName, Guid correlationId)
        {
            return await Task.Factory.StartNew(() =>
            {                
                    var rpcClient = new MQClientWrapper(Channel, queueName, correlationId);
                    var response = rpcClient.CallAndReceive(message);
                    return response;                
            });
        }

        public async void SendCall(string message, string queueName, Guid correlationId)
        {
            await Task.Factory.StartNew(() =>
            {
                var rpcClient = new MQClientWrapper(Channel, queueName, correlationId);
                rpcClient.Call(Encoding.UTF8.GetBytes(message));
            });
        }
    }
}
