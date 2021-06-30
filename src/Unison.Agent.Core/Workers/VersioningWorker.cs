using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Agent.Core.Models.Store;
using Unison.Agent.Core.Utilities;
using Unison.Common.Amqp.DTO;

namespace Unison.Agent.Core.Workers
{
    public class VersioningWorker : ISubscriptionWorker<AmqpApplyVersion>
    {
        private readonly DataStore _store;
        private readonly IAgentConfiguration _agentConfig;
        private readonly ILogger<CacheWorker> _logger;

        public VersioningWorker(DataStore store, IAgentConfiguration agentConfig, ILogger<CacheWorker> logger)
        {
            _store = store;
            _agentConfig = agentConfig;
            _logger = logger;
        }

        public void ProcessMessage(AmqpApplyVersion message)
        {
            Console.WriteLine("This has reached the Cache Version Worker");

            if (message == null)
                return;

            _store.ApplyChanges(message.Entity, message.Version);
        }
    }
}
