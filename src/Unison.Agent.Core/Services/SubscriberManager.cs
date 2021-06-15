﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Services
{
    public class SubscriberManager : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<SubscriberManager> _logger;
        private IEnumerable<IAmqpSubscriber> _subscribers;

        public SubscriberManager(IServiceProvider services, ILogger<SubscriberManager> logger)
        {
            _logger = logger;
            _services = services;
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

                var subscriberInitializer = scope.ServiceProvider.GetRequiredService<IAmqpSubscriberInitializer>();
                _subscribers = subscriberInitializer.Initialize();
            }
        }
    }
}
