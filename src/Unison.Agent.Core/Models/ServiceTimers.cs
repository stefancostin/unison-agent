using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Models
{
    public class ServiceTimers : IDisposable
    {
        public Timer InitializationTimer { get; set; }
        public Timer HeartbeatTimer { get; set; }

        public void Dispose()
        {
            InitializationTimer?.Dispose();
            HeartbeatTimer?.Dispose();
        }
    }
}
