﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Interfaces.Amqp
{
    public interface IAmqpSubscriberFactory
    {
        IEnumerable<IAmqpSubscriber> CreateSubscribers(Dictionary<string, string> exchangeQueueMap);
    }
}
