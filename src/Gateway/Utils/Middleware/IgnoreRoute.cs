using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gateway.Utils.Middleware
{
    public sealed class IgnoreRoute
    {
        private readonly RequestDelegate next;
        private readonly List<string> ignoredRoutes;

        public IgnoreRoute(RequestDelegate next, List<string> routes)
        {
            this.next = next;
            ignoredRoutes = routes;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue)
            {
                foreach (var route in ignoredRoutes) {
                    if(context.Request.Path.Value.Contains(route))
                    {
                        context.Response.StatusCode = 404;
                        return;
                    }
                }
            }

            await next.Invoke(context);
        }
    }
}
