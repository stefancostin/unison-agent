using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Amqp;
using Unison.Agent.Core.Interfaces.Workers;

namespace Unison.Agent.Infrastructure.Amqp
{
    public class AmqpSubscriber : IAmqpSubscriber
    {
        private readonly string _queue;
        private readonly ISubscriptionWorker _worker;
        private readonly IAmqpManagedChannel _channel;
        private readonly ILogger _logger;

        public AmqpSubscriber(string queue, ISubscriptionWorker worker, IAmqpManagedChannel channel, ILogger logger)
        {
            _queue = queue;
            _worker = worker;
            _channel = channel;
            _logger = logger;
        }

        public void Subscribe()
        {
            var consumer = new EventingBasicConsumer(_channel.GetChannel());

            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation(message);
                _worker.ProcessRequest(message);
            };

            _channel.GetChannel().BasicConsume(queue: _queue, autoAck: true, consumer: consumer);
        }

        public void Unsubscribe()
        {
            _channel?.Dispose();
        }

        ~AmqpSubscriber()
        {
            Unsubscribe();
        }
    }
}
