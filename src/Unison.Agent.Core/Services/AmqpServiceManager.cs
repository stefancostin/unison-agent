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
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Services
{
    public class AmqpServiceManager : IHostedService
    {
        private readonly IAgentConfiguration _agentConfig;
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly ILogger<AmqpServiceManager> _logger;
        private readonly IServiceProvider _services;
        private IEnumerable<IAmqpSubscriber> _subscribers;

        public AmqpServiceManager(IServiceProvider services, ILogger<AmqpServiceManager> logger, IAgentConfiguration agentConfig, IAmqpConfiguration amqpConfig)
        {
            _agentConfig = agentConfig;
            _amqpConfig = amqpConfig;
            _logger = logger;
            _services = services;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            InitializeAmqpInfrastructure();

            Parallel.ForEach(_subscribers, subscriber => subscriber.Subscribe());

            SendInitialCconnectionStatus();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var subscriber in _subscribers)
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

        private void SendInitialCconnectionStatus()
        {
            using (var scope = _services.CreateScope())
            {
                var amqpPublisher = scope.ServiceProvider.GetRequiredService<IAmqpPublisher>();
                var exchange = _amqpConfig.Exchanges.Connections;
                var message = new AmqpConnected()
                {
                    Agent = new AmqpAgent() { AgentId = _agentConfig.Id }
                };
                amqpPublisher.PublishMessage(message, exchange);
            }
        }
    }
}
