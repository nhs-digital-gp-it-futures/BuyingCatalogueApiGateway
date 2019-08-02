using Gateway.Models;
using Gateway.Queueing;
using Gateway.Utils.Http;
using Microsoft.Extensions.Configuration;
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
        private Guid CorrelationId;
        private readonly IConfiguration configuration;
        private readonly IDeserializer deserializer;
        private readonly IModel channel;

        public Router(IConfiguration config, IModel rabbitChannel)
        {
            channel = rabbitChannel;

            deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();

            configuration = config;

            string mqRoutesPath = Path.Combine(Environment.CurrentDirectory, "Routes", "MQ");
            string httpRoutesPath = Path.Combine(Environment.CurrentDirectory, "Routes", "Http");

            var mqRoutes = GetRoutes(mqRoutesPath, TransportType.MessageQueue);
            var httpRoutes = GetRoutes(httpRoutesPath, TransportType.Http);

            // Sets the Endpoints list to the list containing MQRoutes and adds the HTTP routes to it
            Endpoints = mqRoutes;
            Endpoints.AddRange(httpRoutes);
        }

        public Router()
        {            
            deserializer = new DeserializerBuilder()
                .WithNamingConvention(new CamelCaseNamingConvention())
                .Build();
        }

        public List<Route> GetRoutes(string path, TransportType transportType)
        {
            var routeFiles = GetFiles(path, "*.yml");

            return ExtractRoutes(transportType, routeFiles, deserializer);
        }

        private static List<Route> ExtractRoutes(TransportType transportType, FileInfo[] routeFiles, IDeserializer deserializer)
        {
            var routes = new List<Route>();

            foreach (var routeFile in routeFiles)
            {
                using (var reader = new StreamReader(routeFile.FullName))
                {
                    var routeInfo = reader.ReadToEnd();
                    var route = deserializer.Deserialize<Route>(routeInfo);
                    route.TransportType = transportType;
                    routes.Add(route);
                }
            }

            return routes;
        }

        internal async Task<ExtractedResponse> RouteRequest(ExtractedRequest request)
        {
            Debug.WriteLine($"body is {request.Body}");

            CorrelationId = Guid.Parse(request.Headers["X-Correlation-Id"]);

            // Organise the path to the final endpoint
            string path = request.Path.ToString();
            string basePath = '/' + path.Split('/')[1];

            // Get the correct endpoint for the route requested
            Route route;
            try
            {
                route = Endpoints.First(r => r.Endpoint.Equals(basePath));
            }
            catch
            {
                return new ExtractedResponse("Unable to locate path", HttpStatusCode.NotFound);
            }

            if (route.Destination.RequiresAuthentication)
            {
                // Here goes the Auth plugin!
            }

            if (route != null)
            {
                if (route.TransportType == TransportType.Http)
                {
                    var apiUrl = configuration.GetConnectionString(route.Destination.ApiName);

                    IRestRequest restRequest = HttpRequestMessageFactory.GenerateRequestMessage(request, apiUrl);

                    return await HttpClientWrapper.SendRequest(restRequest);
                }
                else if (route.TransportType == TransportType.MessageQueue)
                {
                    var response = await new SendMessage(channel).SendAndReceiveCall(JsonConvert.SerializeObject(request),route.Destination.MessageQueue, CorrelationId);

                    return new ExtractedResponse(response, HttpStatusCode.OK);
                }
                else
                {
                    var content = "Transport type not supported";

                    return new ExtractedResponse(content, HttpStatusCode.BadRequest);
                }
            }
            else
            {
                return new ExtractedResponse("An error occurred", HttpStatusCode.RequestTimeout);
            }
        }

        internal FileInfo[] GetFiles(string path, string extension)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            var routeFiles = directory.GetFiles(extension);

            return routeFiles;
        }
    }
}
