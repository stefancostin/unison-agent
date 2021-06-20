using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Exceptions;
using Unison.Agent.Core.Interfaces.Data;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Agent.Core.Models;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Workers
{
    public class SyncWorker : ISubscriptionWorker<AmqpSyncRequest>
    {
        private readonly AgentCache _cache;
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly ILogger<SyncWorker> _logger;

        public SyncWorker(AgentCache cache, IAmqpPublisher publisher, ISQLRepository repository, ILogger<SyncWorker> logger)
        {
            _cache = cache;
            _publisher = publisher;
            _repository = repository;
            _logger = logger;
        }

        public void ProcessMessage(AmqpSyncRequest message)
        {
            Console.WriteLine("This has reached the SyncWorker");

            // Do not start synchronization if we haven't received the cached entities yet
            if (!_cache.Initialized)
                return;

            ValidateMessage(message);
            QuerySchema schema = MapAmqpMessageoQuerySchema(message);

            var differences = new List<Dictionary<string, object>>();

            var cache = _cache.Entities.GetValueOrDefault(schema.Entity)?.Data;
            var result = _repository.Read(schema);

            if (cache == null || !cache.Any())
            {
                // map everything from the db to differences
                // map everything from the db to cache (or do this after commands.apply)
            }    

            var r = result.FirstOrDefault();
            _logger.LogInformation($"{r["Id"]}, {r["Name"]}, {r["Price"]}");

            var response = new AmqpSyncResponse() { QueryResult = result };
            _publisher.PublishMessage(response, "unison.responses");
        }

        private QuerySchema MapAmqpMessageoQuerySchema(AmqpSyncRequest message)
        {
            return new QuerySchema()
            {
                Entity = message.Entity,
                Fields = message.Fields,
                PrimaryKey = message.PrimaryKey
            };
        }

        private void ValidateMessage(AmqpSyncRequest message)
        {
            if (message == null)
                throw new InvalidRequestException("A synchronization request cannot be empty");

            if (string.IsNullOrWhiteSpace(message.Entity))
                throw new InvalidRequestException("An entity name must be provided");

            if (!message.Fields.Any())
                throw new InvalidRequestException("At least one field name must be provided");
        }
    }
}
