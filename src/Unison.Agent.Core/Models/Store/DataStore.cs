using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unison.Agent.Core.Data;
using Unison.Agent.Core.Utilities;

namespace Unison.Agent.Core.Models.Store
{
    /// <summary>
    /// Serves as a single source of truth for the agent.
    /// It is initialized by caching the data from the cloud server.
    /// </summary>
    public class DataStore
    {
        private readonly object _entityLock;

        public DataStore()
        {
            _entityLock = new object();
            Entities = new ConcurrentDictionary<string, StoreDataSet>();
            EntityChanges = new ConcurrentDictionary<string, StoreChanges>();
        }

        public bool Initialized { get; set; }
        public ConcurrentDictionary<string, StoreDataSet> Entities { get; set; }
        public ConcurrentDictionary<string, StoreChanges> EntityChanges { get; set; }

        public void AddEntity(StoreDataSet storeDataSet)
        {
            string entity = storeDataSet.Entity;
            Entities.AddOrUpdate(entity, storeDataSet, (entity, existingStoreDataSet) => storeDataSet);
        }

        public void ApplyChanges(string entity, long version)
        {
            lock (_entityLock)
            {
                StoreDataSet storeEntity = GetStoreEntity(entity);

                if (storeEntity == null)
                    return;

                SyncState changes = GetStoreChanges(entity, version);

                if (changes == null)
                    return;

                string primaryKey = null;
                StoreRecord record = null;

                foreach (KeyValuePair<string, Record> addedRecord in changes.Added.Records)
                {
                    primaryKey = addedRecord.Key;
                    record = addedRecord.Value?.ToStoreRecordModel();
                    storeEntity.Records.AddOrUpdate(primaryKey, record, (primaryKey, existingRecord) => record);
                }

                foreach (KeyValuePair<string, Record> updatedRecord in changes.Updated.Records)
                {
                    primaryKey = updatedRecord.Key;
                    record = updatedRecord.Value?.ToStoreRecordModel();
                    storeEntity.Records.AddOrUpdate(primaryKey, record, (primaryKey, existingRecord) => record);
                }

                foreach (KeyValuePair<string, Record> deletedRecord in changes.Deleted.Records)
                {
                    primaryKey = deletedRecord.Key;
                    storeEntity.Records.Remove(primaryKey, out record);
                }
            }
        }

        public void TrackChanges(SyncState syncState)
        {
            string entity = syncState.Entity;
            long version = syncState.Version;

            lock (_entityLock)
            {
                StoreChanges versionedEntityChanges = EntityChanges.GetOrAdd(entity, (entity) => new StoreChanges());
                versionedEntityChanges.Versions.AddOrUpdate(version, syncState, (version, existingSyncState) => syncState);
            }
        }

        /// <summary>
        /// Retrieves a deep-clone snapshot of the cached entity in a thread-safe manner
        /// </summary>
        /// <param name="entity">The cached entity name</param>
        /// <returns>A deep-clone snapshot of the cached entity</returns>
        public StoreDataSet GetEntitySnapshot(string entity)
        {
            lock (_entityLock)
            {
                return Entities.GetValueOrDefault(entity).Clone();
            }
        }

        private SyncState GetStoreChanges(string entity, long version)
        {
            lock (_entityLock)
            {
                StoreChanges versionedEntityChanges = null;
                EntityChanges.TryGetValue(entity, out versionedEntityChanges);

                if (versionedEntityChanges == null)
                    return null;

                SyncState entityChanges = null;
                versionedEntityChanges.Versions.TryGetValue(version, out entityChanges);

                return entityChanges;
            }
        }

        private StoreDataSet GetStoreEntity(string entity)
        {
            lock (_entityLock)
            {
                StoreDataSet storeEntity = null;
                Entities.TryGetValue(entity, out storeEntity);
                return storeEntity;
            }
        }
    }
}
