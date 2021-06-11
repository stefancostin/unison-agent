using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using Unison.Agent.Core.Interfaces.Amqp;
using Unison.Agent.Core.Models;

namespace Unison.Agent.Infrastructure.Amqp.Client
{
    public class AmqpPublisher : IAmqpPublisher
    {
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly IAmqpManagedChannel _channel;
        private readonly ILogger<AmqpPublisher> _logger;

        private const string exchange = "unison.responses";

        public AmqpPublisher(IAmqpChannelFactory channelFactory, ILogger<AmqpPublisher> logger)
        {
            _channelFactory = channelFactory;
            _channel = channelFactory.CreateManagedChannel();
            _logger = logger;
        }

        public void Publish(AmqpResponse response)
        {
            // Create the Message object, serialize it to JSON and then encode it to bytes
            // var message = new { Name = "Producer", Message = "Hello!" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response));

            _channel.GetChannel().BasicPublish(exchange: exchange,
                                               routingKey: exchange,
                                               basicProperties: null,
                                               body: body);
        }

        // I can have another method for TaskParallel operations that each create a channel
        // Basically, this meants having a method be used for parallel that begins with:
        //   using var channel = _channelFactory.CreateChannel();

    }
}
