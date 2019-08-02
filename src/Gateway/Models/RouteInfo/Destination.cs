using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Gateway.Routing
{
    public class Destination
    {
        [YamlMember(Alias = "requires-authentication", ApplyNamingConventions = false)]
        public bool RequiresAuthentication { get; set; }
        public string Uri { get; set; }
        public string MessageQueue { get; set; }

        [YamlMember(Alias = "api-name", ApplyNamingConventions = false)]
        public string ApiName { get; set; }
    }
}
