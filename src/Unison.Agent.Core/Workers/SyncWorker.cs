﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces;
using Unison.Agent.Core.Interfaces.Amqp;
using Unison.Agent.Core.Interfaces.Data;
using Unison.Agent.Core.Interfaces.Workers;

namespace Unison.Agent.Core.Services.Workers
{
    public class SyncWorker : ISubscriptionWorker
    {
        private readonly IAmqpPublisher _publisher;
        private readonly ISQLRepository _repository;
        private readonly ILogger<SyncWorker> _logger;

        public SyncWorker(IAmqpPublisher publisher, ISQLRepository repository, ILogger<SyncWorker> logger)
        {
            _publisher = publisher;
            _repository = repository;
            _logger = logger;
        }

        public void ProcessRequest(string message)
        {
            Console.WriteLine("This has reached the SyncWorker");
            var result = _repository.Execute("SELECT * FROM Products");

            var r = result.FirstOrDefault();
            _logger.LogInformation($"{r["Id"].ToString()}, {r["Name"].ToString()}, {r["Price"].ToString()}");
        }
    }
}