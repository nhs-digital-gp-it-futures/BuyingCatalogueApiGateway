using FluentAssertions;
using Gateway.Models.Requests;
using Gateway.Http;
using Microsoft.AspNetCore.Http;
using RestSharp;
using Xunit;

namespace Gateway.Tests.UnitTests.Utils
{
    public class HttpRequestMessageHandlerTests
    {
        [Fact]
        public void GenerateRequestMessage()
        {
            var headers = new HeaderDictionary(){
                {"fruit", "banana" },
                {"game", "cards" }
            };

            var extractedRequest = new ExtractedRequest("/endpoint", "bob", "?dave=rodney", headers, "GET");            

            var request = HttpRequestMessageFactory.GenerateRequestMessage(extractedRequest, "http://example.test.com");

            request.Method.Should().Be(Method.GET);
            request.Resource.Should().Be("http://example.test.com/endpoint");            
        }
    }
}
