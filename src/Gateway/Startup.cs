using Gateway.Queueing;
using Gateway.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                .SetBasePath(env.ContentRootPath)
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            RabbitMQConnection connection = new RabbitMQConnection(Configuration.GetConnectionString("MQserver"));
            IConnection channel = connection.CreateConnection();
            IModel rabbitModel = channel.CreateModel();
            RabbitMQHelper helper = new RabbitMQHelper(rabbitModel);

            helper.SetupQueue("audit");

            Router router = new Router(Configuration, rabbitModel);
            app.Run(async (context) =>
            {
                SendMessage sendMessage = new SendMessage(rabbitModel);
                var correlationId = Guid.NewGuid();
                context.Request.Headers.Add("X-Correlation-Id", correlationId.ToString());
                var extractedRequest = new ExtractedRequest(context.Request);

                sendMessage.SendCall(JsonConvert.SerializeObject(extractedRequest), "audit", correlationId);

                var content = await router.RouteRequest(extractedRequest);
                context.Response.Headers.Add("X-Correlation-Id", correlationId.ToString());
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)Enum.Parse(typeof(HttpStatusCode), content.StatusCode.ToString());
                content.Headers = context.Response.Headers;

                sendMessage.SendCall(JsonConvert.SerializeObject(content), "audit", correlationId);

                await context.Response.WriteAsync(content.Body);
            });
        }
    }
}
