using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Data;

namespace Unison.Agent.Core.Models
{
    public class SyncState
    {
        public string Entity { get; set; }
        public long Version { get; set; }
        public DataSet Added { get; set; }
        public DataSet Updated { get; set; }
        public DataSet Deleted { get; set; }

        public bool IsEmpty()
        {
            return (new List<DataSet>() { Added, Updated, Deleted }).All(ds => ds == null || ds.IsEmpty());
        }
    }
}
