using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Agent.Core.Models;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Workers
{
    public class CacheWorker : ISubscriptionWorker<AmqpCache>
    {
        private readonly AgentCache _cache;
        private readonly IAgentConfiguration _agentConfig;
        private readonly ILogger<CacheWorker> _logger;

        public CacheWorker(AgentCache cache, IAgentConfiguration agentConfig, ILogger<CacheWorker> logger)
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

            var products = message.Entities.FirstOrDefault(e => e.Entity == "products");

            _cache.Entities = MapAmqpEntitiesToApplicationEntities(message.Entities);
            _cache.Initialized = true;

            Console.WriteLine("Entities have been cached");
        }

        private ConcurrentDictionary<string, CachedEntity> MapAmqpEntitiesToApplicationEntities(IEnumerable<AmqpCachedEntity> amqpEntities)
        {
            var cachedEntities = new ConcurrentDictionary<string, CachedEntity>();

            if (!amqpEntities.Any())
                return cachedEntities;

            var entities = amqpEntities
                .Where(e => !string.IsNullOrWhiteSpace(e.Entity))
                .Select(e => new CachedEntity()
                {
                    Entity = e.Entity,
                    Data = new ConcurrentBag<ConcurrentDictionary<string, object>>(e.Data?.Select(d => new ConcurrentDictionary<string, object>(d))),
                    UpdatedAt = DateTime.Now
                }); ;

            foreach (var entity in entities)
            {
                cachedEntities.AddOrUpdate(entity.Entity, entity, (key, existingEntity) => entity);
            }

            return cachedEntities;
        }
    }
}
