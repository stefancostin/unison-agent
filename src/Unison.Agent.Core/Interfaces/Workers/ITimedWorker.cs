﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Models;

namespace Unison.Agent.Core.Interfaces.Workers
{
    public interface ITimedWorker
    {
        public void Start(object state);
    }
}
