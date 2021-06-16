using Microsoft.Extensions.DependencyInjection;
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
                var reconnectionsSubscriber = InitializeReconnectionsSubscriber(scope);
                var syncSubscriber = InitializeSyncSubscriber(scope);

                var subscribers = new List<IAmqpSubscriber>();

                subscribers.Add(reconnectionsSubscriber);
                subscribers.Add(syncSubscriber);

                return subscribers;
            }
        }

        private IAmqpSubscriber InitializeReconnectionsSubscriber(IServiceScope scope)
        {
            var reconnectionsWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpReconnect>>();
            var queue = _amqpConfig.Queues.CommandReconnect;
            return _subscriberFactory.CreateSubscriber(queue, reconnectionsWorker);
        }

        private IAmqpSubscriber InitializeSyncSubscriber(IServiceScope scope)
        {
            var syncWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpSyncRequest>>();
            var queue = _amqpConfig.Queues.CommandSync;
            return _subscriberFactory.CreateSubscriber(queue, syncWorker);
        }
    }
}
