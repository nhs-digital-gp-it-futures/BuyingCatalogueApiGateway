using FluentAssertions;
using Gateway.Routing;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using System.IO;
using Xunit;

namespace Gateway.Tests.UnitTests.Models
{
    public class ExtractedRequestTest
    {
        readonly Mock<HttpRequest> mockRequest;

        public ExtractedRequestTest()
        {
            var headers = new HeaderDictionary()
            {
                { "bob", "dave" }
            };

            var bodyContent = "{\"test\":\"banana\"}";

            var body = new MemoryStream();
            var sw = new StreamWriter(body);
            var json = JsonConvert.SerializeObject(bodyContent);
            sw.Write(json);
            sw.Flush();

            body.Position = 0;

            mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Headers).Returns(headers);
            mockRequest.Setup(x => x.Method).Returns("GET");
            mockRequest.Setup(x => x.Body).Returns(body);
            mockRequest.Setup(x => x.Path).Returns("/testing123");
            mockRequest.Setup(x => x.Query["bob"]).Returns("dave");

        }


        [Fact]
        public void ExtractRequest_FromHttp()
        {
            var headers = new HeaderDictionary()
            {
                { "bob", "dave" }
            };

            var extractedRequest = new ExtractedRequest(mockRequest.Object);

            extractedRequest.Body.Should().Be(JsonConvert.SerializeObject("{\"test\":\"banana\"}"));
            extractedRequest.Method.Should().Be("GET");
            extractedRequest.Path.Should().Be("/testing123");
            extractedRequest.Headers.Should().BeEquivalentTo(headers);
        }

        [Fact]
        public void ExtractRequest_IndividualSections()
        {
            var headers = new HeaderDictionary()
            {
                { "bob", "dave" }
            };

            var extractedRequest = new ExtractedRequest("/testing123", JsonConvert.SerializeObject("{\"test\":\"banana\"}"), "?bob=dave", headers, "GET");

            extractedRequest.Body.Should().Be(JsonConvert.SerializeObject("{\"test\":\"banana\"}"));
            extractedRequest.Method.Should().Be("GET");
            extractedRequest.Path.Should().Be("/testing123");
            extractedRequest.Headers.Should().BeEquivalentTo(headers);
        }
    }
}
