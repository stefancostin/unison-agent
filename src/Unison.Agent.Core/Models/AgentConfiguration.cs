using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;

namespace Unison.Agent.Core.Models
{
    public class AgentConfiguration : IAgentConfiguration
    {
        public string Id { get; set; }
    }
}
