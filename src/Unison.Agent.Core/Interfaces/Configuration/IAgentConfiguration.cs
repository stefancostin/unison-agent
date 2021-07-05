using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Interfaces.Configuration
{
    public interface IAgentConfiguration
    {
        string InstanceId { get; set; }
        int HeartbeatTimer { get; set; }
    }
}
