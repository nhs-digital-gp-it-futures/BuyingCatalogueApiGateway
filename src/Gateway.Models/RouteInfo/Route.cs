namespace Gateway.Models.RouteInfo
{
    public enum TransportType
    {
        MessageQueue,
        Http
    }

    public sealed class Route
    {
        public string Endpoint { get; set; }
        public TransportType TransportType { get; set; }
        public Destination Destination { get; set; }
    }
}
