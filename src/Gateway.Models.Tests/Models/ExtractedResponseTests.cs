using FluentAssertions;
using Gateway.Models.Responses;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Net;
using Xunit;

namespace Gateway.Models.Tests.Models
{
    public class ExtractedResponseTests
    {
        public ExtractedResponseTests()
        {
                      
        }
        
        [Fact]
        public void ExtractedResponse_CanGenerateFromHttpResponse()
        {
            var bodyContent = "{\"test\":\"banana\"}";

            var body = new MemoryStream();
            var sw = new StreamWriter(body);
            var json = JsonConvert.SerializeObject(bodyContent);
            sw.Write(json);
            sw.Flush();

            body.Position = 0;

            var responseMock = new Mock<Microsoft.AspNetCore.Http.HttpResponse>();
            responseMock.Setup(s => s.StatusCode).Returns(200);
            responseMock.Setup(s => s.Body).Returns(body);

            var response = new ExtractedResponse(responseMock.Object);

            response.Body.Should().Be(json);
            response.StatusCode.Should().Be((HttpStatusCode)200);
        }

        [Fact]
        public void ExtractedResponse_CanBeGeneratedFromRestResponse()
        {
            var bodyContent = "{\"test\":\"banana\"}";
            var responseMock = new Mock<IRestResponse>();
            responseMock.Setup(s => s.StatusCode).Returns(HttpStatusCode.NotFound);
            responseMock.Setup(s => s.Content).Returns(bodyContent);

            var response = new ExtractedResponse(responseMock.Object);
            response.Body.Should().Be(bodyContent);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
