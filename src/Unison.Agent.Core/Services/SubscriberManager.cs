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
        private readonly IServiceProvider _services;
        private readonly IEnumerable<IAmqpSubscriber> _subscribers;
        private readonly ILogger<SubscriberManager> _logger;

        public SubscriberManager(IAmqpSubscriberFactory subscriberFactory, IServiceProvider services, IConfiguration config, ILogger<SubscriberManager> logger)
        {
            _config = config;
            _logger = logger;
            _services = services;
            _subscribers = subscriberFactory.CreateSubscribers();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeAmqpInfrastructure();

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

        private void InitializeAmqpInfrastructure()
        {
            using (var scope = _services.CreateScope())
            {
                var amqpInfrastructureInitializer = scope.ServiceProvider.GetRequiredService<IAmqpInfrastructureInitializer>();
                amqpInfrastructureInitializer.Initialize();
            }
        }
    }
}
