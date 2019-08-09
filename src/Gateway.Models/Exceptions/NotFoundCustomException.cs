using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Gateway.Models.Exceptions
{
    public sealed class NotFoundCustomException : CustomException
    {
        public NotFoundCustomException(string message, string description) : base(message, description, (int)HttpStatusCode.NotFound)
        {
        }
    }
}
