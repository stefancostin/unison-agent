using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Common.Amqp.Constants;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Infrastructure.Amqp
{
    public class AmqpSubscriberInitializer : IAmqpSubscriberInitializer
    {
        private readonly IServiceProvider _services;
        private readonly IAmqpSubscriberFactory _subscriberFactory;
        private readonly Dictionary<string, string> _consumerExchangeQueueMap;

        public AmqpSubscriberInitializer(IServiceProvider services, IAmqpSubscriberFactory subscriberFactory, IAmqpInitializationState initializationState)
        {
            _services = services;
            _subscriberFactory = subscriberFactory;
            _consumerExchangeQueueMap = initializationState.ConsumerExchangeQueueMap;
        }

        public IEnumerable<IAmqpSubscriber> Initialize()
        {
            using (var scope = _services.CreateScope())
            {
                //var connectionsSubscriber = CreateConnectionsSubscriber(scope);
                var syncSubscriber = InitializeSyncSubscriber(scope);

                var subscribers = new List<IAmqpSubscriber>();
                //subscribers.Add(connectionsSubscriber);
                subscribers.Add(syncSubscriber);

                return subscribers;
            }
        }

        //private IAmqpSubscriber InitializeConnectionsSubscriber(IServiceScope scope)
        //{
        //    // TODO: Need to differentiate between the two classes of ISubscriptionWorker
        //    var connectionsWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker>();
        //    var queue = _consumerExchangeQueueMap[AmqpExchangeNames.Connections];
        //    return _subscriberFactory.CreateSubscriber(queue, connectionsWorker);
        //}

        private IAmqpSubscriber InitializeSyncSubscriber(IServiceScope scope)
        {
            var syncWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker<AmqpMessage>>();
            var queue = _consumerExchangeQueueMap[AmqpExchangeNames.Commands];
            return _subscriberFactory.CreateSubscriber(queue, syncWorker);
        }
    }
}
