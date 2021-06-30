using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Models.Store
{
    /// <summary>
    /// Handles an agent store <b>entity data set</b> in a thread-safe manner.
    /// The store serves as a single source of truth for the agent (cloud data cache).
    /// </summary>
    public class StoreDataSet
    {
        private readonly object _versionLock;

        public StoreDataSet(string entity, string primaryKey)
        {
            Entity = entity;
            PrimaryKey = primaryKey;
            Records = new ConcurrentDictionary<string, StoreRecord>();
        }

        public StoreDataSet(string entity, string primaryKey, long version) : this(entity, primaryKey)
        {
            Version = version;
        }

        public string Entity { get; set; }
        public string PrimaryKey { get; set; }
        public long Version { get; set; }
        public ConcurrentDictionary<string, StoreRecord> Records { get; set; }

        public StoreRecord GetRecord(string primaryKey)
        {
            return Records.GetValueOrDefault(primaryKey);
        }

        public bool IsEmpty()
        {
            if (Records == null)
                return true;

            return Records.Any((KeyValuePair<string, StoreRecord> record) => record.Value != null && !record.Value.IsEmpty());
        }
    }
}
