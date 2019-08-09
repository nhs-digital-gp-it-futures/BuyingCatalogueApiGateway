using FluentAssertions;
using Gateway.Models.Exceptions;
using Gateway.Utils.Middleware;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Gateway.Tests.UnitTests.Utils.Middleware
{
    public class ErrorHandlerTests
    {
        [Fact]
        public async void ErrorHandler_CustomExceptions_NotFound()
        {
            var errorHandler = new ErrorHandler((innerHttpContext) => throw new NotFoundCustomException("message", "description"));

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await errorHandler.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<dynamic>(streamText);

            //Assert
            var message = (string)objResponse.Message;
            var description = (string)objResponse.Description;

            message.Should().Be("message");
            description.Should().Be("description");

            context.Response.StatusCode.Should().Be(404);
        }

        [Fact]
        public async void ErrorHandler_CustomExceptions_BadRequest()
        {
            var errorHandler = new ErrorHandler((innerHttpContext) => throw new BadRequestCustomException("message", "description"));

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await errorHandler.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<dynamic>(streamText);

            //Assert
            var message = (string)objResponse.Message;
            var description = (string)objResponse.Description;

            message.Should().Be("message");
            description.Should().Be("description");

            context.Response.StatusCode.Should().Be(400);
        }

        [Fact]
        public async void ErrorHandler_CustomExceptions_NotAuthorized()
        {
            var errorHandler = new ErrorHandler((innerHttpContext) => throw new NotAuthorizedCustomException("message", "description"));

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await errorHandler.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<dynamic>(streamText);

            //Assert
            var message = (string)objResponse.Message;
            var description = (string)objResponse.Description;

            message.Should().Be("message");
            description.Should().Be("description");

            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async void ErrorHandler_CustomExceptions_CustomException()
        {
            var errorHandler = new ErrorHandler((innerHttpContext) => throw new CustomException("message", "description", 202));

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await errorHandler.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();
            var objResponse = JsonConvert.DeserializeObject<dynamic>(streamText);

            //Assert
            var message = (string)objResponse.Message;
            var description = (string)objResponse.Description;

            message.Should().Be("message");
            description.Should().Be("description");

            context.Response.StatusCode.Should().Be(202);
        }

        [Fact]
        public async void ErrorHandler_NoError()
        {
            var errorHandler = new ErrorHandler((innerHttpContext) => Task.FromResult(0));

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await errorHandler.Invoke(context);

            context.Response.StatusCode.Should().Be(200);
        }
    }
}
