﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Infrastructure.Amqp
{
    public class AmqpSubscriberInitializer : IAmqpSubscriberInitializer
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IServiceProvider _services;
        private readonly IAmqpSubscriberFactory _subscriberFactory;

        public AmqpSubscriberInitializer(IServiceProvider services, IAmqpConfiguration amqpConfig, IAmqpSubscriberFactory subscriberFactory)
        {
            _amqpConfig = amqpConfig;
            _services = services;
            _subscriberFactory = subscriberFactory;
        }

        public IEnumerable<IAmqpSubscriber> Initialize()
        {
            using (var scope = _services.CreateScope())
            {
                var cacheSubscriber = InitializeCacheSubscriber(scope);
                var reconnectSubscriber = InitializeReconnectSubscriber(scope);
                var syncSubscriber = InitializeSyncSubscriber(scope);

                var subscribers = new List<IAmqpSubscriber>();

                subscribers.Add(cacheSubscriber);
                subscribers.Add(reconnectSubscriber);
                subscribers.Add(syncSubscriber);

                return subscribers;
            }
        }

        private IAmqpSubscriber InitializeCacheSubscriber(IServiceScope scope)
        {
            var cacheWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpCache>>();
            var queue = _amqpConfig.Queues.CommandCache;
            return _subscriberFactory.CreateSubscriber(queue, cacheWorker);
        }

        private IAmqpSubscriber InitializeReconnectSubscriber(IServiceScope scope)
        {
            var reconnectWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpReconnect>>();
            var queue = _amqpConfig.Queues.CommandReconnect;
            return _subscriberFactory.CreateSubscriber(queue, reconnectWorker);
        }

        private IAmqpSubscriber InitializeSyncSubscriber(IServiceScope scope)
        {
            var syncWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpSyncRequest>>();
            var queue = _amqpConfig.Queues.CommandSync;
            return _subscriberFactory.CreateSubscriber(queue, syncWorker);
        }
    }
}
