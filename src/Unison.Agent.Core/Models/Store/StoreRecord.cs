using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Models.Store
{
    /// <summary>
    /// Handles an agent store <b>record</b> in a thread-safe manner.
    /// The store serves as a single source of truth for the agent (cloud data cache).
    /// </summary>
    public class StoreRecord
    {
        public StoreRecord()
        {
            Fields = new ConcurrentDictionary<string, StoreField>();
        }

        public ConcurrentDictionary<string, StoreField> Fields { get; set; }

        public void AddField(StoreField field)
        {
            Fields.AddOrUpdate(field.Name, field, (fieldName, existingField) => field);
        }

        public void AddField(string fieldName, Type fieldType, object fieldValue)
        {
            StoreField newField = new StoreField(fieldName, fieldType, fieldValue);
            Fields.AddOrUpdate(fieldName, newField, (fieldName, existingField) => newField);
        }

        public StoreField GetField(string fieldName)
        {
            return Fields.GetValueOrDefault(fieldName);
        }
    }
}
