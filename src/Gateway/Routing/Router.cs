using Gateway.Http.PublicInterfaces;
using Gateway.Models.Exceptions;
using Gateway.Models.Requests;
using Gateway.Models.Responses;
using Gateway.Models.RouteInfo;
using Gateway.MQ.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TransportType = Gateway.Models.RouteInfo.TransportType;

namespace Gateway.Routing
{
    public sealed class Router
    {
        internal List<Route> Endpoints { get; }
        public IConfiguration Configuration { get; set; }

        private readonly IMessageClient messageClient;
        private readonly IHttpClient httpClient;

        public Router(IMessageClient messageClient, IHttpClient httpClient)
        {
            this.messageClient = messageClient;
            this.httpClient = httpClient;

            string mqRoutesPath = Path.Combine(Environment.CurrentDirectory, "Routes", "MQ");
            string httpRoutesPath = Path.Combine(Environment.CurrentDirectory, "Routes", "Http");

            var mqRoutes = GetRoutes(mqRoutesPath, TransportType.MessageQueue, messageClient);
            var httpRoutes = GetRoutes(httpRoutesPath, TransportType.Http);

            // Sets the Endpoints list to the list containing MQRoutes and adds the HTTP routes to it
            Endpoints = mqRoutes;
            Endpoints.AddRange(httpRoutes);
        }

        public async Task<ExtractedResponse> RouteRequest(ExtractedRequest request)
        {
            // Organise the path to the final endpoint            
            string basePath = '/' + request.Path.Split('/')[1];

            ExtractedResponse response = new ExtractedResponse();

            // Get the correct endpoint for the route requested
            Route route;

            try
            {
                route = Endpoints.First(r => r.Endpoint.Equals(basePath));
            }
            catch
            {
                throw new NotFoundCustomException("Route not found", $"Route matching {basePath} not found");
            }

            if (route.Destination.RequiresAuthentication)
            {
                // Auth stuff would go here, throwing exception for now
                throw new NotAuthorizedCustomException("Unauthorized", "You do not have permission to access this resource");
            }

            if (route.TransportType == TransportType.Http)
            {
                response = await httpClient.SendRequest(request, route);
            }
            else if (route.TransportType == TransportType.MessageQueue)
            {
                messageClient.PushMessageIntoQueue(request.GetJsonBytes(), route.Destination.MessageQueue);

                response.Body = "{message: \"Message client does not support responses at the moment\"}";
                response.StatusCode = HttpStatusCode.OK;
            }

            return response;
        }

        public static List<Route> GetRoutes(string path, TransportType transportType, IMessageClient helper = null)
        {
            var routeFiles = GetFiles(path, "*.yml");

            return ExtractRoutes(transportType, routeFiles, helper);
        }

        private static List<Route> ExtractRoutes(TransportType transportType, FileInfo[] routeFiles, IMessageClient helper = null)
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

                    // Sets up queue if it doesn't exist
                    if (helper != null)
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
