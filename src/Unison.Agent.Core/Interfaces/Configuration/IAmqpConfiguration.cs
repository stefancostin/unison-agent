using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Models;

namespace Unison.Agent.Core.Interfaces.Configuration
{
    public interface IAmqpConfiguration
    {
        AmqpCredentials Credentials { get; set; }
        AmqpExchanges Exchanges { get; set; }
        AmqpCommands Commands { get; set; }
        AmqpQueues Queues { get; set; }
    }
}
