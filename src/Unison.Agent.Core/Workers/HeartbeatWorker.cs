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
    public class HeartbeatWorker : ITimedWorker
    {
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpPublisher _publisher;
        private readonly ILogger<HeartbeatWorker> _logger;

        public HeartbeatWorker(IAmqpConfiguration amqpConfig, IAmqpPublisher publisher, ILogger<HeartbeatWorker> logger)
        {
            _amqpConfig = amqpConfig;
            _publisher = publisher;
            _logger = logger;
        }

        public void Start(object state)
        {
            var correlationId = Guid.NewGuid().ToString();
            _logger.LogDebug($"CorrelationId: {correlationId}. Sending heartbeat to cloud server.");

            var heartbeat = new AmqpHeartbeat()
            {
                Agent = new AmqpAgent() { InstanceId = state?.ToString() },
                CorrelationId = correlationId
            };

            _publisher.PublishMessage(heartbeat, _amqpConfig.Exchanges.Heartbeat);
        }
    }
}
