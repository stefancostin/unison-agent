using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Amqp;

namespace Unison.Agent.Core.Services
{
    public class SubscriberManager : IHostedService
    {
        private readonly IConfiguration _config;
        private readonly IEnumerable<IAmqpSubscriber> _subscribers;
        private readonly ILogger<SubscriberManager> _logger;

        public SubscriberManager(IAmqpSubscriberFactory subscriberFactory, IConfiguration config, ILogger<SubscriberManager> logger)
        {
            _config = config;
            _logger = logger;
            _subscribers = subscriberFactory.CreateSubscribers();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var subscriber in _subscribers)
            {
               subscriber.Subscribe();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var subscriber in _subscribers)
            {
                subscriber.Unsubscribe();
            }

            return Task.CompletedTask;
        }

    }
}
