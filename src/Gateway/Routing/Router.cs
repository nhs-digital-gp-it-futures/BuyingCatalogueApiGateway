using Gateway.Models;
using Gateway.Queueing;
using Gateway.Utils.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Gateway.Routing
{
    public sealed class Router
    {
        internal List<Route> Endpoints { get; }        
        public IConfiguration Configuration { get; set; }
                
        private readonly IRabbitMQConnectionFactory connectionFactory;
        private readonly IConnection Connection;
        private readonly RabbitMQHelper rabbitMQHelper;
        private Guid CorrelationId;

        public Router(IOptionsMonitor<RabbitMQConnectionDetails> connectionDetails)
        {
            this.connectionFactory = new RabbitMQConnection(connectionDetails.CurrentValue);

            Connection = connectionFactory.CreateConnection();

            rabbitMQHelper = new RabbitMQHelper(Connection, this.connectionFactory.GetExchangeName());

            string mqRoutesPath = Path.Combine(Environment.CurrentDirectory, "Routes", "MQ");
            string httpRoutesPath = Path.Combine(Environment.CurrentDirectory, "Routes", "Http");

            var mqRoutes = GetRoutes(mqRoutesPath, TransportType.MessageQueue, rabbitMQHelper);
            var httpRoutes = GetRoutes(httpRoutesPath, TransportType.Http);

            // Sets the Endpoints list to the list containing MQRoutes and adds the HTTP routes to it
            Endpoints = mqRoutes;
            Endpoints.AddRange(httpRoutes);
        }

        public Router()
        {            
        }

        internal async Task<ExtractedResponse> RouteRequest(ExtractedRequest request)
        {
            rabbitMQHelper.PushAuditMessage(request.GetJsonBytes());

            CorrelationId = Guid.Parse(request.Headers["X-Correlation-Id"]);

            // Organise the path to the final endpoint            
            string basePath = '/' + request.Path.Split('/')[1];

            ExtractedResponse response = new ExtractedResponse();

            // Get the correct endpoint for the route requested
            Route route = null;
            try
            {
                route = Endpoints.First(r => r.Endpoint.Equals(basePath));
            }
            catch
            {
                response.Body = "Unable to locate path";
                response.StatusCode = HttpStatusCode.NotFound;                
            }            

            if (route != null)
            {
                bool authorized = route.Destination.RequiresAuthentication ? /* Stuff should happen here to authorize peeps*/ false : true ;
                if (route.Destination.RequiresAuthentication)
                {
                    // Put the auth stuff here
                }
                else
                {
                    authorized = true;
                }

                if (authorized)
                {
                    if (route.TransportType == TransportType.Http)
                    {
                        var apiUrl = Configuration.GetConnectionString(route.Destination.ApiName);

                        // Does the converting for endpoint routing properly
                        var endPath = request.Path.Split('/');
                        endPath[1] = route.Destination.Uri;
                        request.Path = string.Join('/', endPath);

                        IRestRequest restRequest = HttpRequestMessageFactory.GenerateRequestMessage(request, apiUrl);

                        response = await HttpClientWrapper.SendRequest(restRequest);
                    }
                    else if (route.TransportType == TransportType.MessageQueue)
                    {
                        

                        response.Body = "";
                        response.StatusCode = HttpStatusCode.OK;
                    }
                    else
                    {
                        response.Body = "Transport type not supported";
                        response.StatusCode = HttpStatusCode.BadRequest;
                    }
                }
            }
            else
            {
                response.Body = "An error occurred";
                response.StatusCode = HttpStatusCode.RequestTimeout;
            }

            response.Headers.Add("X-Correlation-Id", CorrelationId.ToString());            
            rabbitMQHelper.PushAuditMessage(response.GetJsonBytes());

            return response;
        }

        public static List<Route> GetRoutes(string path, TransportType transportType, RabbitMQHelper helper = null)
        {
            var routeFiles = GetFiles(path, "*.yml");

            return ExtractRoutes(transportType, routeFiles, helper);
        }

        private static List<Route> ExtractRoutes(TransportType transportType, FileInfo[] routeFiles, RabbitMQHelper helper = null)
        {
            var routes = new List<Route>();

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            foreach (var routeFile in routeFiles)
            {
                using (var reader = new StreamReader(routeFile.FullName))
                {
                    var routeInfo = reader.ReadToEnd();
                    var route = deserializer.Deserialize<Route>(routeInfo);
                    route.TransportType = transportType;
                    routes.Add(route);

                    // Sets up Rabbit queue if it doesn't exist
                    if(helper != null)
                    {
                        helper.SetupQueue(route.Destination.MessageQueue);
                    }
                }
            }

            return routes;
        }

        internal static FileInfo[] GetFiles(string path, string extension)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            var routeFiles = directory.GetFiles(extension);

            return routeFiles;
        }
    }
}
