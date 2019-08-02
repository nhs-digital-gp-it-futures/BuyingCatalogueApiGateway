namespace Gateway.Routing
{
    public enum TransportType
    {
        MessageQueue,
        Http
    }

    public class Route
    {
        public string Endpoint { get; set; }
        public TransportType TransportType { get; set; }
        public Destination Destination { get; set; }
    }
}
