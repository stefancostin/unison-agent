using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Models.Store
{
    /// <summary>
    /// Handles an agent store <b>field</b> in a thread-safe manner.
    /// The store serves as a single source of truth for the agent (cloud data cache).
    /// </summary>
    public class StoreField
    {
        private string _name;
        private Type _type;
        private object _value;

        private object _nameLock;
        private object _typeLock;
        private object _valueLock;

        public StoreField()
        {
            _nameLock = new object();
            _typeLock = new object();
            _valueLock = new object();
        }

        public StoreField(string name, Type type, object value) : this()
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public string Name
        {
            get
            {
                lock (_nameLock)
                {
                    return _name;
                }
            }
            set 
            {
                lock (_nameLock)
                {
                    _name = value;
                }
            }
        }

        public Type Type
        {
            get
            {
                lock (_typeLock)
                {
                    return _type;
                }
            }
            set
            {
                lock (_typeLock)
                {
                    _type = value;
                }
            }
        }
        
        public object Value
        {
            get
            {
                lock (_valueLock)
                {
                    return _value;
                }
            }
            set
            {
                lock (_valueLock)
                {
                    _value = value;
                }
            }
        }

    }
}
