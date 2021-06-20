using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Models
{
    public class AgentCache
    {
        public AgentCache()
        {
            Entities = new ConcurrentDictionary<string, CachedEntity>();
        }

        public bool Initialized { get; set; }
        public ConcurrentDictionary<string, CachedEntity> Entities { get; set; }
    }

    public class CachedEntity
    {
        public string Entity { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ConcurrentBag<ConcurrentDictionary<string, object>> Data { get; set; }
    }
}
