using Gateway.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Http;

namespace Gateway.Utils
{
    public static class ResponseMessageHandler
    {
        public static ExtractedResponse ConstructResponseMessage(HttpResponse response)
        {
            var responseMessage = new ExtractedResponse(response);

            return responseMessage;
        }        
    }
}
