﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Interfaces.Workers
{
    public interface ISubscriptionWorker
    {
        public void ProcessRequest(string message);
    }
}
