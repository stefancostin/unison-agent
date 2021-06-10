using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Amqp;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Models;

namespace Unison.Agent.Infrastructure.Amqp
{
    public class AmqpInfrastructureInitializer : IAmqpInfrastructureInitializer
    {
        private readonly IAgentConfiguration _agentConfig;
        private readonly IAmqpConfiguration _amqpConfig;
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly Dictionary<string, string> _consumerExchangeQueueMap;

        public AmqpInfrastructureInitializer(IAgentConfiguration agentConfig, IAmqpConfiguration amqpConfig, IAmqpChannelFactory channelFactory)
        {
            _agentConfig = agentConfig;
            _amqpConfig = amqpConfig;
            _channelFactory = channelFactory;
            _consumerExchangeQueueMap = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Initialize()
        {
            using (var channel = _channelFactory.CreateUnmanagedChannel())
            {
                BindToCommandsExchange(channel);
                BindToConnectionsExchange(channel);
            }
            return _consumerExchangeQueueMap;
        }


        private void BindToCommandsExchange(IModel channel)
        {
            var agentId = _agentConfig.Id;
            var exchange = _amqpConfig.Exchanges.Commands;
            var commands = new List<string>() { "sync" };

            foreach (string command in commands)
            {
                var queue = $"{exchange}.{command}.{agentId}";

                channel.QueueDeclare(queue: queue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var agentSpecificRoutingKey = $"{exchange}.{command}.{agentId}";

                channel.QueueBind(queue: queue,
                                  exchange: exchange,
                                  routingKey: agentSpecificRoutingKey,
                                  arguments: null);

                var genericRoutingKey = $"{exchange}.{command}";

                channel.QueueBind(queue: queue,
                     exchange: exchange,
                     routingKey: genericRoutingKey,
                     arguments: null);

                _consumerExchangeQueueMap.Add(AmqpExchanges.Commands, queue);
            }
        }

        private void BindToConnectionsExchange(IModel channel)
        {
            var agentId = _agentConfig.Id;
            var exchange = _amqpConfig.Exchanges.Connections;

            var queue = $"{exchange}.{agentId}";

            channel.QueueDeclare(queue: queue,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.QueueBind(queue: queue,
                 exchange: exchange,
                 routingKey: "",
                 arguments: null);

            _consumerExchangeQueueMap.Add(AmqpExchanges.Connections, queue);
        }
    }
}
