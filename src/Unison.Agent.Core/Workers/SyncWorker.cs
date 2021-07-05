using Microsoft.Extensions.DependencyInjection;
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
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly ILogger<SyncWorker> _logger;

        public SyncWorker(DataStore dataStore, IAgentConfiguration agentConfig, IAmqpConfiguration amqpConfig, IAmqpPublisher publisher, ISQLRepository repository, ILogger<SyncWorker> logger)
        {
            _dataStore = dataStore;
            _agentConfig = agentConfig;
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _repository = repository;
            _logger = logger;
        }

        public void ProcessMessage(AmqpSyncRequest message)
        {
            // Don't start synchronization if the cached entities haven't been received yet 
            if (!_dataStore.Initialized)
                return;

            ValidateMessage(message);

            string correlationId = message.CorrelationId;
            _logger.LogDebug($"CorrelationId: {correlationId}. Received cache request for {message.Entity}.");

            QuerySchema schema = message.ToQuerySchema();

            SyncState syncState = Synchronize(schema);

            //if (syncState.IsEmpty())
            //{
            //    _logger.LogDebug($"CorrelationId: {correlationId}. No changes found for {message.Entity}.");
            //    return;
            //}

            LogSyncState(syncState, correlationId);

            _dataStore.TrackChanges(syncState);

            PublishSyncState(syncState, correlationId);
        }

        private SyncState Synchronize(QuerySchema schema)
        {
            StoreDataSet cachedEntitySnapshot = _dataStore.GetEntitySnapshot(schema.Entity);

            SyncState syncState = schema.ToSyncState();
            syncState.Version = cachedEntitySnapshot.Version;

            DataSet cloudEntity = cachedEntitySnapshot.ToDataSetModel();
            DataSet localEntity = _repository.Read(schema);

            // TODO: Log out the count fo the retrieved entities

            if (cloudEntity == null || cloudEntity.IsEmpty())
            {
                syncState.Added = localEntity;
                return syncState;
            }

            // Filter out any empty records that could have been incorrectly stored due to errors in data
            cloudEntity.Records = cloudEntity.Records.Where(r => r.Value != null && !r.Value.IsEmpty()).ToDictionary(r => r.Key, r => r.Value);
            localEntity.Records = localEntity.Records.Where(r => r.Value != null && !r.Value.IsEmpty()).ToDictionary(r => r.Key, r => r.Value);

            foreach (KeyValuePair<string, Record> localRecord in localEntity.Records)
            {
                // Add the new database record in case it doesn't exist in the cloud cache
                if (!cloudEntity.Records.ContainsKey(localRecord.Key))
                {
                    syncState.Added.Records.Add(localRecord.Key, localRecord.Value);
                }
                // Check for updated fields in case the records have the same primary keys
                else if (cloudEntity.Records.ContainsKey(localRecord.Key))
                {
                    if (!cloudEntity.GetRecord(localRecord.Key).Equals(localEntity.GetRecord(localRecord.Key)))
                    {
                        syncState.Updated.Records.Add(localRecord.Key, localRecord.Value);
                    }
                }
            }

            foreach (KeyValuePair<string, Record> cloudRecord in cloudEntity.Records)
            {
                // Find the records that existed in the cloud but have been meanwhile deleted from the local database
                if (!localEntity.Records.ContainsKey(cloudRecord.Key))
                {
                    syncState.Deleted.Records.Add(cloudRecord.Key, cloudRecord.Value);
                }
            }

            return syncState;
        }

        private AmqpSyncResponse CreateSyncResponse(AmqpSyncState syncState)
        {
            return new AmqpSyncResponse()
            {
                Agent = new AmqpAgent() { InstanceId = _agentConfig.InstanceId },
                State = syncState
            };
        }

        private void LogSyncState(SyncState syncState, string correlationId)
        {
            _logger.LogInformation($"CorrelationId: {correlationId}. " +
                $"{syncState.Added.Records.Count()} records added,  " +
                $"{syncState.Updated.Records.Count()} records updated and " +
                $"{syncState.Deleted.Records.Count()} records deleted " +
                $"for {syncState.Entity}, on version {syncState.Version}.");
        }

        private void PublishSyncState(SyncState syncState, string correlationId)
        {
            AmqpSyncResponse response = CreateSyncResponse(syncState.ToAmqpSyncState());
            response.CorrelationId = correlationId;
            string exchange = _amqpConfig.Exchanges.Response;
            _publisher.PublishMessage(response, exchange);
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

        // Used only for debugging.
        private void PrintFirstRecord(DataSet dataSet)
        {
            var r = dataSet.Records?.FirstOrDefault().Value;
            _logger.LogInformation($"{r.Fields["Id"]?.Value}, {r.Fields["Name"]?.Value}, {r.Fields["Price"]?.Value}");
        }
    }
}
