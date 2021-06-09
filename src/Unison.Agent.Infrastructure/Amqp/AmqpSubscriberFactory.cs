using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Amqp;
using Unison.Agent.Core.Interfaces.Workers;

namespace Unison.Agent.Infrastructure.Amqp
{
    public class AmqpSubscriberFactory : IAmqpSubscriberFactory
    {
        // TODO: Read the queues from the config files
        private const string queue = "demo-queue";

        private readonly IConfiguration _config;
        private readonly IServiceProvider _services;
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly ILoggerFactory _loggerFactory;

        public AmqpSubscriberFactory(IAmqpChannelFactory channelFactory, ILoggerFactory loggerFactory, IServiceProvider services, IConfiguration config)
        {
            _services = services;
            _channelFactory = channelFactory;
            _loggerFactory = loggerFactory;
            _config = config;
        }

        public IEnumerable<IAmqpSubscriber> CreateSubscribers()
        {
            using (var scope = _services.CreateScope())
            {
                var syncWorker = scope.ServiceProvider.GetRequiredService<ISubscriptionWorker>();

                var connectionsSubscriber = CreateSubscriber(queue, syncWorker);

                var subscribers = new List<IAmqpSubscriber>();
                subscribers.Add(connectionsSubscriber);

                return subscribers;
            }
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
