using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Models.Store
{
    public class StoreChanges
    {
        public StoreChanges()
        {
            Versions = new ConcurrentDictionary<long, SyncState>();
        }

        public ConcurrentDictionary<long, SyncState> Versions { get; set; }
    }
}
