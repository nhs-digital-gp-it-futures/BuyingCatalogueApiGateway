using FluentAssertions;
using Gateway.Utils.Http;
using System.Net;
using Xunit;

namespace Gateway.Tests.UnitTests.Utils
{
    public class ConvertStatusCodeTests
    {
        [Theory]
        [InlineData(HttpStatusCode.OK, 200)]
        [InlineData(HttpStatusCode.Created, 201)]
        [InlineData(HttpStatusCode.Accepted, 202)]
        public void ConvertStatusCodes_Success(HttpStatusCode code, int expectedValue)
        {
            HttpUtils.ConvertStatusCode(code).Should().Be(expectedValue);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest, 400)]
        [InlineData(HttpStatusCode.RequestTimeout, 408)]
        [InlineData(HttpStatusCode.InternalServerError, 500)]
        public void ConvertStatusCodes_Failure(HttpStatusCode code, int expectedValue)
        {
            HttpUtils.ConvertStatusCode(code).Should().Be(expectedValue);
        }
    }
}
