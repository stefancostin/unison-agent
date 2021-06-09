using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Infrastructure.Amqp
{
    public interface IAmqpChannelFactory
    {
        IAmqpManagedChannel CreateManagedChannel();
        IModel CreateUnmanagedChannel();
    }
}
