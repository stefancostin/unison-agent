using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Data;
using Unison.Agent.Core.Exceptions;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Data;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Agent.Core.Models;
using Unison.Agent.Core.Models.Store;
using Unison.Agent.Core.Utilities;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Workers
{
    public class SyncWorker : ISubscriptionWorker<AmqpSyncRequest>
    {
        private readonly DataStore _dataStore;
        private readonly IAgentConfiguration _agentConfig;
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly ILogger<SyncWorker> _logger;

        public SyncWorker(DataStore dataStore, IAgentConfiguration agentConfig, IAmqpPublisher publisher, ISQLRepository repository, ILogger<SyncWorker> logger)
        {
            _dataStore = dataStore;
            _agentConfig = agentConfig;
            _publisher = publisher;
            _repository = repository;
            _logger = logger;
        }

        public void ProcessMessage(AmqpSyncRequest message)
        {
            Console.WriteLine("This has reached the SyncWorker");

            // Do not start synchronization if we haven't received the cached entities yet
            if (!_dataStore.Initialized)
                return;

            ValidateMessage(message);
            QuerySchema schema = message.ToQuerySchema();

            StartSync(schema);

            // TODO: Remove this unnecessary read after debugging
            var result = _repository.Read(schema);

            var r = result.Records?.FirstOrDefault().Value;
            _logger.LogInformation($"{r.Fields["Id"]?.Value}, {r.Fields["Name"]?.Value}, {r.Fields["Price"]?.Value}");

            var response = new AmqpSyncResponse()
            {
                Agent = new AmqpAgent() { AgentId = _agentConfig.Id },
                State = new AmqpSyncState() { Added = result.ToAmqpDataSetModel() }
            };
            _publisher.PublishMessage(response, "unison.responses");
        }

        private AmqpSyncState StartSync(QuerySchema schema)
        {
            var syncState = new AmqpSyncState();
            syncState.Added = new AmqpDataSet(schema.Entity, schema.PrimaryKey);
            syncState.Updated = new AmqpDataSet(schema.Entity, schema.PrimaryKey);
            syncState.Deleted = new AmqpDataSet(schema.Entity, schema.PrimaryKey);

            DataSet cloudEntity = _dataStore.GetEntitySnapshot(schema.Entity);
            DataSet dbEntity = _repository.Read(schema);

            if (cloudEntity == null || !cloudEntity.Records.Any())
            {
                syncState.Added = dbEntity.ToAmqpDataSetModel();
                return syncState;
            }

            foreach (KeyValuePair<string, Record> record in dbEntity.Records)
            {
                // Add the new database record in case it doesn't exist in the cloud cache
                if (!cloudEntity.Records.ContainsKey(record.Key) && record.Value != null)
                {
                    syncState.Added.Records.Add(record.Key, record.Value.ToAmqpRecordModel());
                }
                // Check for updated fields in case the records have the same primary keys
                else if (cloudEntity.Records.ContainsKey(record.Key))
                {
                    if (!cloudEntity.GetRecord(record.Key).Equals(dbEntity.GetRecord(record.Key)))
                    {
                        syncState.Updated.Records.Add(record.Key, record.Value.ToAmqpRecordModel());
                    }
                }

                // Since remove is an O(1) operation it's useful to optimize the search for deleted records
                cloudEntity.Records.Remove(record.Key);
            }

            // The remaining records existed in the cloud but they have been deleted from the local database
            syncState.Deleted = cloudEntity.ToAmqpDataSetModel();

            return syncState;
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
