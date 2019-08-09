using System;

namespace Gateway.Models.Exceptions
{
    public class CustomException : Exception
    {
        public int StatusCode { get; }
        public string Description { get; }

        public CustomException(string message, string description, int code) : base(message)
        {
            StatusCode = code;
            Description = description;
        }
    }
}
