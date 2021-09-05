using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Exceptions;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Common.Amqp.DTO;

namespace Unison.Agent.Core.Workers
{
    public class ConfigurationWorker : ISubscriptionWorker<AmqpAgentConfiguration>
    {
        private readonly IAgentConfiguration _agentConfiguration;
        private readonly ILogger<ConfigurationWorker> _logger;

        public ConfigurationWorker(IAgentConfiguration agentConfiguration, ILogger<ConfigurationWorker> logger)
        {
            _agentConfiguration = agentConfiguration;
            _logger = logger;
        }

        public void ProcessMessage(AmqpAgentConfiguration message)
        {
            ValidateMessage(message);
            _logger.LogInformation($"CorrelationId: {message.CorrelationId}. Received new configuration. " +
                $"Heartbeat now set at {message.HeartbeatTimer} seconds.");
            _agentConfiguration.HeartbeatTimer = message.HeartbeatTimer;
        }

        private void ValidateMessage(AmqpAgentConfiguration message)
        {
            if (message == null)
                throw new InvalidRequestException("A configuration message cannot be empty");

            if (message.HeartbeatTimer < 0)
                throw new InvalidRequestException("A valid heartbeat timer must be provided");
        }
    }
}
