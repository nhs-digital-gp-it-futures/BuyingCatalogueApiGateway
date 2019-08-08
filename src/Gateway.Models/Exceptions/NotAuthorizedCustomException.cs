using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Gateway.Models.Exceptions
{
    public sealed class NotAuthorizedCustomException : CustomException
    {
        public NotAuthorizedCustomException(string message, string description) : base(message, description, (int)HttpStatusCode.Unauthorized)
        {
        }
    }
}
