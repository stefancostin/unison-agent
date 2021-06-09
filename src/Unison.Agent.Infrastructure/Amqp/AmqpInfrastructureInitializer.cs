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

        public AmqpInfrastructureInitializer(IAgentConfiguration agentConfig, IAmqpConfiguration amqpConfig, IAmqpChannelFactory channelFactory)
        {
            _agentConfig = agentConfig;
            _amqpConfig = amqpConfig;
            _channelFactory = channelFactory;
        }

        public void Initialize()
        {
            using (var channel = _channelFactory.CreateUnmanagedChannel())
            {
                foreach (var exchange in _amqpConfig.Exchanges.Subscribe)
                {

                    if (exchange.Type.ToLower() == AmqpTopics.Fanout)
                        InitFanoutExchangeInfrastructure(channel, exchange);

                    else if (exchange.Type.ToLower() == AmqpTopics.Topic)
                        InitTopicExchangeInfrastructure(channel, exchange);
                }
            }
        }

        private void InitFanoutExchangeInfrastructure(IModel channel, AmqpSubscribeExchange subscribeExchange)
        {
            var agentId = _agentConfig.Id;
            var exchange = subscribeExchange.Name;

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
        }

        private void InitTopicExchangeInfrastructure(IModel channel, AmqpSubscribeExchange subscribeExchange)
        {
            var agentId = _agentConfig.Id;
            var exchange = subscribeExchange.Name;

            foreach (string command in subscribeExchange.Commands)
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
            }
        }
    }
}
