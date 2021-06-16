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
    public class ConnectionWorker : ISubscriptionWorker<AmqpReconnect>
    {
        private readonly IAgentConfiguration _agentConfig;
        private readonly IAmqpPublisher _amqpPublisher;
        private readonly ILogger<ConnectionWorker> _logger;

        public ConnectionWorker(IAgentConfiguration agentConfig, IAmqpPublisher publisher, ILogger<ConnectionWorker> logger)
        {
            _agentConfig = agentConfig;
            _amqpPublisher = publisher;
            _logger = logger;
        }

        public void ProcessMessage(AmqpReconnect receivedMessage)
        {
            Console.WriteLine("This has reached the ConnectionWorker");

            var message = new AmqpConnected() { 
                Agent = new AmqpAgent() { AgentId = _agentConfig.Id } 
            };

            _amqpPublisher.PublishMessage(message, "unison.connections");
        }
    }
}
