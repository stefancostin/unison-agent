using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Exceptions;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Agent.Core.Models.Store;
using Unison.Agent.Core.Utilities;
using Unison.Common.Amqp.DTO;

namespace Unison.Agent.Core.Workers
{
    public class CacheWorker : ISubscriptionWorker<AmqpCache>
    {
        private readonly DataStore _store;
        private readonly ILogger<CacheWorker> _logger;

        public CacheWorker(DataStore store, ILogger<CacheWorker> logger)
        {
            _store = store;
            _logger = logger;
        }

        public void ProcessMessage(AmqpCache message)
        {
            ValidateMessage(message);

            string correlationId = message.CorrelationId;

            _logger.LogInformation($"CorrelationId: {correlationId}. Received {message.Entities.Count()} entities to cache.");

            foreach (AmqpDataSet entityCache in message.Entities)
            {
                _store.AddEntity(entityCache.ToStoreEntityModel());
            }

            _store.Initialized = true;

            string cachedEntityNames = string.Join(", ", message.Entities.Select(e => e.Entity));
            _logger.LogInformation($"CorrelationId: {correlationId}. Entities {cachedEntityNames} have been cached.");
        }

        private void ValidateMessage(AmqpCache message)
        {
            if (message == null || message.Entities == null)
                throw new InvalidRequestException("A connection/cache request cannot be empty");

            if (message.Entities.Any(e => string.IsNullOrWhiteSpace(e.Entity) || e.Version < 0))
                throw new InvalidRequestException("Valid entity names and versions must be provided");
        }
    }
}
