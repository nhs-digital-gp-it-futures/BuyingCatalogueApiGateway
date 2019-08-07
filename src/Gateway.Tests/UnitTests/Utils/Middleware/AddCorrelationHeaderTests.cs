using FluentAssertions;
using Gateway.Utils.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gateway.Tests.UnitTests.Utils.Middleware
{
    public class AddCorrelationHeaderTests
    {
        readonly AddHeaders middleware;

        public AddCorrelationHeaderTests()
        {
            middleware = new AddHeaders(next: (innerHttpContext) => Task.FromResult(0)); 
        }

        [Fact]
        public async void Middleware_SetsCorrelationHeader()
        {
            var context = new DefaultHttpContext();

            await middleware.Invoke(context);

            context.Request.Headers.Should().ContainKey("X-Correlation-Id");
            context.Response.Headers.Should().ContainKey("X-Correlation-Id");
        }

        [Fact]
        public async void Middleware_ResponseCorrelationMatchesRequestCorrelation()
        {
            var context = new DefaultHttpContext();

            await middleware.Invoke(context);

            var correlation = context.Request.Headers["X-Correlation-Id"];

            context.Response.Headers["X-Correlation-Id"].Should().BeEquivalentTo(correlation);
        }
    }
}
