using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Amqp;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;

namespace Unison.Agent.Infrastructure.Amqp
{
    public class AmqpSubscriberFactory : IAmqpSubscriberFactory
    {
        private readonly IServiceProvider _services;
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly ILoggerFactory _loggerFactory;

        public AmqpSubscriberFactory(IAmqpChannelFactory channelFactory, ILoggerFactory loggerFactory, IServiceProvider services)
        {
            _services = services;
            _channelFactory = channelFactory;
            _loggerFactory = loggerFactory;
        }

        public IEnumerable<IAmqpSubscriber> CreateSubscribers(Dictionary<string, string> consumerExchangeQueueMap)
        {
            using (var scope = _services.CreateScope())
            {
                //var connectionsSubscriber = CreateConnectionsSubscriber(scope, exchangeQueueMap);
                var syncSubscriber = CreateSyncSubscriber(scope, consumerExchangeQueueMap);

                var subscribers = new List<IAmqpSubscriber>();
                //subscribers.Add(connectionsSubscriber);
                subscribers.Add(syncSubscriber);

                return subscribers;
            }
        }

        private IAmqpSubscriber CreateConnectionsSubscriber(IServiceScope scope, Dictionary<string, string> consumerExchangeQueueMap)
        {
            // TODO: Need to differentiate between the two classes of ISubscriptionWorker
            var connectionsWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker>();
            var queue = consumerExchangeQueueMap[AmqpExchanges.Connections];
            return CreateSubscriber(queue, connectionsWorker);
        }

        private IAmqpSubscriber CreateSyncSubscriber(IServiceScope scope, Dictionary<string, string> consumerExchangeQueueMap)
        {
            var syncWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker>();
            var queue = consumerExchangeQueueMap[AmqpExchanges.Commands];
            return CreateSubscriber(queue, syncWorker);
        }


        private IAmqpSubscriber CreateSubscriber(string queue, ISubscriptionWorker worker)
        {
            return new AmqpSubscriber(queue, worker, _channelFactory.CreateManagedChannel(), _loggerFactory.CreateLogger(CreateLoggerName(queue)));
        }

        private string CreateLoggerName(string queue)
        {
            return $"{nameof(IAmqpSubscriber)}-{queue}";
        }
    }
}
