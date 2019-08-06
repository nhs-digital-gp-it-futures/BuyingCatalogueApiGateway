using Gateway.Queueing;
using Gateway.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.IO;
using System.Net;

namespace Gateway
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddOptions();

            var section = Configuration.GetSection("RabbitMQ");
            services.Configure<RabbitMQConnectionDetails>(section);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IOptionsMonitor<RabbitMQConnectionDetails> connDetails)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Router router = new Router(connDetails)
            {
                // Configuration included to help with HTTP API calls
                Configuration = Configuration
            };

            app.Use(async (context, next) =>
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
                await next.Invoke();
            });

            app.Run(async (context) =>
            {   
                var extractedRequest = new ExtractedRequest(context.Request);

                var content = await router.RouteRequest(extractedRequest);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)content.StatusCode;
                content.Headers = context.Response.Headers;                

                await context.Response.WriteAsync(content.Body);
            });
        }
    }
}
