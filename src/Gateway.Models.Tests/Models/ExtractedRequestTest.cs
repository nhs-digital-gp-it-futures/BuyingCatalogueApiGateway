using FluentAssertions;
using Gateway.Models.Requests;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using System.IO;
using Xunit;

namespace Gateway.Tests.UnitTests.Models
{
    public class ExtractedRequestTest
    {
        [Fact]
        public void ExtractRequest_FromHttp()
        {
            var headers = new HeaderDictionary()
            {
                { "bob", "dave" }
            };

            var bodyContent = "this is a test";

            var body = new MemoryStream();
            var sw = new StreamWriter(body);            
            sw.Write(bodyContent);
            sw.Flush();

            body.Position = 0;

            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Headers).Returns(headers);
            mockRequest.Setup(x => x.Method).Returns("GET");
            mockRequest.Setup(x => x.Body).Returns(body);
            mockRequest.Setup(x => x.Path).Returns("/testing123");
            mockRequest.Setup(x => x.Query["bob"]).Returns("dave");

            var extractedRequest = new ExtractedRequest(mockRequest.Object);

            extractedRequest.Body.Should().Be(bodyContent);
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
