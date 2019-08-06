using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Utils.Middleware
{
    public class AddHeaders
    {
        private readonly RequestDelegate next;

        public AddHeaders(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = Guid.NewGuid();
            // Add a header to the request containing a correlation ID. If that header is already present, extract it for later use
            if (context.Request.Headers.ContainsKey("X-Correlation-Id"))
            {
                correlationId = Guid.Parse(context.Request.Headers["X-Correlation-Id"]);
            }
            else
            {
                context.Request.Headers.Add("X-Correlation-Id", correlationId.ToString());
            }

            if (!context.Response.Headers.ContainsKey("X-Correlation-Id"))
            {
                context.Response.Headers.Add("X-Correlation-Id", correlationId.ToString());
            }
            context.Response.ContentType = "application/json";

            await next.Invoke(context);
        }
    }
}
