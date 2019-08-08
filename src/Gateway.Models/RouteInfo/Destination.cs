using YamlDotNet.Serialization;

namespace Gateway.Models.RouteInfo
{
    public sealed class Destination
    {
        [YamlMember(Alias = "requires-authentication", ApplyNamingConventions = false)]
        public bool RequiresAuthentication { get; set; }
        public string Uri { get; set; }
        public string MessageQueue { get; set; }

        [YamlMember(Alias = "api-name", ApplyNamingConventions = false)]
        public string ApiName { get; set; }
    }
}
