using Gateway.MQ.Common;
using Gateway.MQ.Interfaces;
using Gateway.MQ.Rabbit;
using Gateway.Routing;
using Gateway.Utils.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;

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

            var section = Configuration.GetSection("MessageQueueSettings").Get<ConnectionDetails>();
            services.AddSingleton(MQConnectionFactory.GetMessageClient(section));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMessageClient client)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }            

            Router router = new Router(client)
            {
                // Configuration included to help with HTTP API calls
                Configuration = Configuration
            };


            // Ignore requests to specified endpoints
            var ignoredRoutes = new List<string>()
            {
                "favicon.ico"
            };

            app.UseMiddleware<IgnoreRoute>(ignoredRoutes);
            app.UseMiddleware<AddHeaders>();

            app.Run(async (context) =>
            {
                var extractedRequest = new ExtractedRequest(context.Request);

                var content = await router.RouteRequest(extractedRequest);
                context.Response.StatusCode = (int)content.StatusCode;
                content.Headers = context.Response.Headers;

                await context.Response.WriteAsync(content.Body);
            });
        }
    }
}
