﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Interfaces.Workers
{
    public interface ISubscriptionWorker<T> : IAmqpSubscriptionWorker<T> { }
}
