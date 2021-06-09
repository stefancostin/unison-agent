using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;

namespace Unison.Agent.Core.Models
{
    public class AmqpConfiguration : IAmqpConfiguration
    {
        public AmqpCredentials Credentials { get; set; }
        public AmqpExchanges Exchanges { get; set; }
    }

    public class AmqpCredentials
    {
        public string Hostname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AmqpExchanges
    {
        public AmqpPublishExchanges Publish { get; set; }
        public IEnumerable<AmqpSubscribeExchange> Subscribe { get; set; }
    }

    public class AmqpPublishExchanges
    {
        public string Heartbeat { get; set; }
        public string Response { get; set; }
    }

    public class AmqpSubscribeExchange
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public IEnumerable<string> Commands { get; set; }
    }
}
