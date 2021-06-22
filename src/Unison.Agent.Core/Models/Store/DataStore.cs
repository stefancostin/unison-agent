using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Models.Store
{
    /// <summary>
    /// Serves as a single source of truth for the agent.
    /// It is initialized by caching the data from the cloud server.
    /// </summary>
    public class DataStore
    {
        public DataStore()
        {
            Entities = new ConcurrentDictionary<string, StoreDataSet>();
        }

        public bool Initialized { get; set; }
        public ConcurrentDictionary<string, StoreDataSet> Entities { get; set; }

        public void AddEntity(StoreDataSet storeDataSet)
        {
            var entity = storeDataSet.Entity;
            Entities.AddOrUpdate(entity, storeDataSet, (entity, existingStoreDataSet) => storeDataSet);
        }

        public StoreDataSet GetEntity(string entity)
        {
            return Entities.GetValueOrDefault(entity);
        }
    }
}
