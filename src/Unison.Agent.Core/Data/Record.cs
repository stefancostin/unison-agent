using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Data
{
    public class Record : IEquatable<Record>
    {
        public Record()
        {
            Fields = new Dictionary<string, Field>();
        }

        public IDictionary<string, Field> Fields { get; set; }

        public void AddField(Field field)
        {
            Fields.Add(field.Name, field);
        }

        public void AddField(string fieldName, Type fieldType, object fieldValue)
        {
            Fields.Add(fieldName, new Field(fieldName, fieldType, fieldValue));
        }

        public bool Equals(Record other)
        {
            if (other == null)
                return false;

            foreach (KeyValuePair<string, Field> field in Fields)
            {
                string fieldName = field.Key;
                if (!field.Value.Equals(other.Fields[fieldName]))
                    return false;
            }

            return true;
        }

        public bool IsEmpty()
        {
            if (Fields == null)
                return true;

            return !Fields.Any();
        }
    }
}
