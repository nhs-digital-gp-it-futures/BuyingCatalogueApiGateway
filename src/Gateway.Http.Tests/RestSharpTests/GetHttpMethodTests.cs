using FluentAssertions;
using Gateway.Http;
using RestSharp;
using System;
using System.Net.Http;
using Xunit;

namespace Gateway.Tests.UnitTests.Utils
{
    public class GetHttpMethodTests
    {
        [Theory]
        [InlineData("GET", Method.GET)]
        [InlineData("POST", Method.POST)]
        [InlineData("PUT", Method.PUT)]
        [InlineData("DELETE", Method.DELETE)]
        public void GetHttpMethods_UsingString(string method, Method expectedMethod)
        {
            HttpUtils.GetHttpMethod(method).Should().Be(expectedMethod);
        }

        [Theory]
        [InlineData("PATCH")]
        [InlineData("BOB")]
        public void GetHttpMethods_InvalidMethodThowsError(string invalidMethod)
        {
            Action act = () => HttpUtils.GetHttpMethod(invalidMethod);

            act.Should().Throw<ArgumentException>().WithMessage($"Method not supported: { invalidMethod }");         
        }

        [Fact]        
        public void GetHttpMethods_UsingHttpMethod()
        {
            HttpUtils.GetHttpMethod(HttpMethod.Get).Should().Be(Method.GET);
        }
    }
}
