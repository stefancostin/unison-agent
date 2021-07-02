using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Models;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Infrastructure.Amqp
{
    public class AmqpInfrastructureInitializer : IAmqpInfrastructureInitializer
    {
        private readonly IAgentConfiguration _agentConfig;
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpChannelFactory _channelFactory;

        public AmqpInfrastructureInitializer(IAgentConfiguration agentConfig, IAmqpConfiguration amqpConfig, IAmqpChannelFactory channelFactory)
        {
            _agentConfig = agentConfig;
            _amqpConfig = amqpConfig;
            _channelFactory = channelFactory;

            amqpConfig.Queues = new AmqpQueues();
        }

        public void Initialize()
        {
            using (var channel = _channelFactory.CreateUnmanagedChannel())
            {
                BindApplyQueueToCommandsExchange(channel);
                BindCacheQueueToCommandsExchange(channel);
                BindReconnectQueueToCommandsExchange(channel);
                BindSyncQueueToCommandsExchange(channel);
            }
        }

        private void BindCacheQueueToCommandsExchange(IModel channel)
        {
            var instanceId = _agentConfig.InstanceId;
            var command = _amqpConfig.Commands.Cache;
            var exchange = _amqpConfig.Exchanges.Commands;

            var queue = $"{exchange}.{command}.{instanceId}";

            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var agentSpecificRoutingKey = $"{exchange}.{command}.{instanceId}";

            channel.QueueBind(queue: queue,
                              exchange: exchange,
                              routingKey: agentSpecificRoutingKey,
                              arguments: null);

            var genericRoutingKey = $"{exchange}.{command}";

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: genericRoutingKey,
                 arguments: null);

            _amqpConfig.Queues.CommandCache = queue;
        }

        private void BindApplyQueueToCommandsExchange(IModel channel)
        {
            var instanceId = _agentConfig.InstanceId;
            var command = _amqpConfig.Commands.ApplyVersion;
            var exchange = _amqpConfig.Exchanges.Commands;

            var queue = $"{exchange}.{command}.{instanceId}";

            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var agentSpecificRoutingKey = $"{exchange}.{command}.{instanceId}";

            channel.QueueBind(queue: queue,
                              exchange: exchange,
                              routingKey: agentSpecificRoutingKey,
                              arguments: null);

            var genericRoutingKey = $"{exchange}.{command}";

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: genericRoutingKey,
                 arguments: null);

            _amqpConfig.Queues.CommandApplyVersion = queue;
        }

        private void BindReconnectQueueToCommandsExchange(IModel channel)
        {
            var instanceId = _agentConfig.InstanceId;
            var command = _amqpConfig.Commands.Reconnect;
            var exchange = _amqpConfig.Exchanges.Commands;

            var queue = $"{exchange}.{command}.{instanceId}";

            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var agentSpecificRoutingKey = $"{exchange}.{command}.{instanceId}";

            channel.QueueBind(queue: queue,
                              exchange: exchange,
                              routingKey: agentSpecificRoutingKey,
                              arguments: null);

            var genericRoutingKey = $"{exchange}.{command}";

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: genericRoutingKey,
                 arguments: null);

            _amqpConfig.Queues.CommandReconnect = queue;
        }

        private void BindSyncQueueToCommandsExchange(IModel channel)
        {
            var instanceId = _agentConfig.InstanceId;
            var command = _amqpConfig.Commands.Sync;
            var exchange = _amqpConfig.Exchanges.Commands;

            var queue = $"{exchange}.{command}.{instanceId}";

            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var agentSpecificRoutingKey = $"{exchange}.{command}.{instanceId}";

            channel.QueueBind(queue: queue,
                              exchange: exchange,
                              routingKey: agentSpecificRoutingKey,
                              arguments: null);

            var genericRoutingKey = $"{exchange}.{command}";

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: genericRoutingKey,
                 arguments: null);

            _amqpConfig.Queues.CommandSync = queue;
        }

    }
}
