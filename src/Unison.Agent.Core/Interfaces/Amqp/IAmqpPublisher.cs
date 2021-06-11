using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Models;

namespace Unison.Agent.Core.Interfaces.Amqp
{
    public interface IAmqpPublisher
    {
        void Publish(AmqpResponse response);
    }
}
