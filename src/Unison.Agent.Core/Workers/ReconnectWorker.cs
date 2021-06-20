using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Workers
{
    public class ReconnectWorker : ISubscriptionWorker<AmqpReconnect>
    {
        private readonly IAgentConfiguration _agentConfig;
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpPublisher _publisher;
        private readonly ILogger<ReconnectWorker> _logger;

        public ReconnectWorker(IAgentConfiguration agentConfig, IAmqpConfiguration amqpConfig, IAmqpPublisher publisher, ILogger<ReconnectWorker> logger)
        {
            _agentConfig = agentConfig;
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _logger = logger;

        }

        public void ProcessMessage(AmqpReconnect _)
        {
            var exchange = _amqpConfig.Exchanges.Connections;
            var message = new AmqpConnected()
            {
                Agent = new AmqpAgent() { AgentId = _agentConfig.Id }
            };
            _publisher.PublishMessage(message, exchange);
        }
    }
}
