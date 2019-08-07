using Gateway.Models.Responses;
using Microsoft.AspNetCore.Http;

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
