using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Gateway.Models.Exceptions
{
    public sealed class BadRequestCustomException : CustomException
    {
        public BadRequestCustomException(string message, string description) : base(message, description, (int)HttpStatusCode.BadRequest)
        {
        }
    }
}
