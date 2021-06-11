using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Interfaces.Amqp
{
    /// <summary>
    /// Subscribes to an exchange using its own RabbitMQ channel.
    /// </summary>
    public interface IAmqpSubscriber
    {
        void Subscribe();
        void Unsubscribe();
    }
}
