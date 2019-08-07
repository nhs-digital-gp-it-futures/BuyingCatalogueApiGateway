using FluentAssertions;
using Gateway.Utils;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using System.Net;
using System.Text;
using Xunit;

namespace Gateway.Tests.UnitTests.Utils
{
    public class ResponseMessageHandlerTest
    {
        [Fact]
        public void GenerateResponseMessage()
        {
            var content = "This is the content";
            var body = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var statusCode = HttpStatusCode.OK;

            var mockedRequest = new Mock<HttpResponse>();
            mockedRequest.Setup(s => s.Body).Returns(body);
            mockedRequest.Setup(s => s.StatusCode).Returns(200);

            var response = ResponseMessageHandler.ConstructResponseMessage(mockedRequest.Object);

            response.Body.Should().Be(content);
            response.StatusCode.Should().Be(statusCode);
        }
    }
}
