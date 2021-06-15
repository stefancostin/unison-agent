using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Data;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Services.Workers
{
    public class HeartbeatWorker : ITimedWorker
    {
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly ILogger<HeartbeatWorker> _logger;

        public HeartbeatWorker(IAmqpPublisher publisher, ISQLRepository repository, ILogger<HeartbeatWorker> logger)
        {
            _publisher = publisher;
            _repository = repository;
            _logger = logger;
        }

        public void Start(object state)
        {
            _logger.LogInformation("Hello from Job");
            //_amqpClient.Publish("Hello from the other side");

            _repository.Execute("SELECT * FROM Products");
        }
    }
}
