using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;
using Unison.Agent.Core.Interfaces.Amqp;

namespace Unison.Agent.Infrastructure.Amqp
{
    public class AmqpPublisher : IAmqpPublisher
    {
        private readonly IAmqpChannelFactory _channelFactory;
        private readonly IAmqpManagedChannel _channel;

        private const string queue = "demo-queue";

        public AmqpPublisher(IAmqpChannelFactory channelFactory)
        {
            _channelFactory = channelFactory;
            _channel = channelFactory.CreateManagedChannel();
        }

        public void Publish(string message)
        {
            // Create the Message object, serialize it to JSON and then encode it to bytes
            // var message = new { Name = "Producer", Message = "Hello!" };
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            _channel.GetChannel().BasicPublish(exchange: "",
                                               routingKey: queue,
                                               basicProperties: null,
                                               body: body);
        }

        // I can have another method for TaskParallel operations that each create a channel
        // Basically, this meants having a method be used for parallel that begins with:
        //   using var channel = _channelFactory.CreateChannel();

    }
}
