using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Infrastructure.Amqp
{
    static class AmqpTopics
    {
        public static string Direct { get { return "direct";  } }
        public static string Fanout { get { return "fanout"; } }

        public static string Headers { get { return "headers"; } }
        public static string Topic { get { return "topic"; } }

    }
}
