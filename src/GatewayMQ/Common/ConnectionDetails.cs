namespace Gateway.MQ.Common
{
    public sealed class ConnectionDetails
    {
        public string HostName { get; set; }
        public string ExchangeName { get; set; }
        public string MQProvider { get; set; }
    }
}
