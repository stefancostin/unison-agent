using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Data;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Common.Amqp.DTO;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Core.Workers
{
    public class SyncWorker : ISubscriptionWorker<AmqpSyncRequest>
    {
        private readonly IAmqpPublisher _amqpPublisher;
        private readonly ISQLRepository _repository;
        private readonly ILogger<SyncWorker> _logger;

        public SyncWorker(IAmqpPublisher publisher, ISQLRepository repository, ILogger<SyncWorker> logger)
        {
            _amqpPublisher = publisher;
            _repository = repository;
            _logger = logger;

        }

        public void ProcessMessage(AmqpSyncRequest message)
        {
            Console.WriteLine("This has reached the SyncWorker");
            var result = _repository.Execute(message.Query);

            var r = result.FirstOrDefault();
            _logger.LogInformation($"{r["Id"]}, {r["Name"]}, {r["Price"]}");

            var response = new AmqpSyncResponse() { QueryResult = result };
            _amqpPublisher.PublishResponse(response, "unison.responses");
        }
    }
}
