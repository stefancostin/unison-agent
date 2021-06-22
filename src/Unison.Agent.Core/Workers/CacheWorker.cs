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
    public class CacheWorker : ISubscriptionWorker<AmqpCache>
    {
        private readonly DataStore _cache;
        private readonly IAgentConfiguration _agentConfig;
        private readonly ILogger<CacheWorker> _logger;

        public CacheWorker(DataStore cache, IAgentConfiguration agentConfig, ILogger<CacheWorker> logger)
        {
            _cache = cache;
            _agentConfig = agentConfig;
            _logger = logger;
        }

        public void ProcessMessage(AmqpCache message)
        {
            Console.WriteLine("This has reached the CacheWorker");

            if (message == null)
                return;

            var products = message.Entities.FirstOrDefault(e => e.Entity == "Products");

            if (products == null)
                return;

            _cache.AddEntity(products.ToStoreEntityModel());
            _cache.Initialized = true;

            Console.WriteLine("Entities have been cached");
        }
    }
}
