﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Models
{
    public class AmqpResponse
    {
        public IEnumerable<Dictionary<string, object>> QueryResult { get; set; }
    }
}
