using FluentAssertions;
using Gateway.Utils.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gateway.Tests.UnitTests.Utils.Middleware
{
    public class IgnoreRouteTests
    {
        readonly IgnoreRoute middleware;

        public IgnoreRouteTests()
        {
            middleware = new IgnoreRoute(next: (initialContext) => Task.FromResult(0), new List<string> { "favicon.ico" });           
        }

        [Fact]
        public async void Middleware_IgnoresRouteCorrectly()
        {
            var context = new DefaultHttpContext();
            context.Request.Path = "/favicon.ico";
            
            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(404);
        }

        [Fact]
        public async void Middleware_DoesNotIgnoreValidRoutes()
        {
            var context = new DefaultHttpContext();
            context.Request.Path = "/bob?test=123";

            await middleware.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
        }
    }
}
