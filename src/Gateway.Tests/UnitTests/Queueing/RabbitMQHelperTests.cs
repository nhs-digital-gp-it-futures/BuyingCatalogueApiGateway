using FluentAssertions;
using Gateway.Queueing;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Gateway.Tests.UnitTests.Queueing
{
    public class RabbitMQHelperTests
    {        
        readonly IModel model;

        public RabbitMQHelperTests()
        {
            var connFactory = new ConnectionFactory();
            var conn = connFactory.CreateConnection();
            model = conn.CreateModel();            
        }

        [Fact]
        public void RabbitMQ_CanSetupQueue()
        {
            var helper = new RabbitMQHelper(model);
            helper.SetupQueue("bob");
        }
    }
}
