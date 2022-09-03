using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Agent.Core.Models;

namespace Unison.Agent.Core.Services
{
    public class TimedServiceManager : BackgroundService
    {
        private readonly TimeSpan _initTimeout;
        private readonly IAgentConfiguration _agentConfig;
        private readonly IServiceProvider _services;
        private readonly ILogger<TimedServiceManager> _logger;
        private readonly ServiceTimers _timers;

        public TimedServiceManager(IAgentConfiguration agentConfig, IServiceProvider services, ServiceTimers timers, ILogger<TimedServiceManager> logger)
        {
            _initTimeout = TimeSpan.FromSeconds(5);
            _agentConfig = agentConfig;
            _logger = logger;
            _services = services;
            _timers = timers;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timers.InitializationTimer = new Timer(ExecuteServices, null, _initTimeout, Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        protected void ExecuteServices(object state) {
            _logger.LogInformation("Starting timeed services...");

            using (var scope = _services.CreateScope())
            {
                var heartbeatWorker = scope.ServiceProvider.GetRequiredService<ITimedWorker>();
                var heartbeatTimer = _agentConfig.HeartbeatTimer > 0 ? _agentConfig.HeartbeatTimer : (int)TimeSpan.FromSeconds(10).TotalSeconds;
                _timers.HeartbeatTimer = new Timer(heartbeatWorker.Start, _agentConfig.InstanceId, TimeSpan.Zero, TimeSpan.FromSeconds(heartbeatTimer));
            }
        }

        ~TimedServiceManager()
        {
            _timers?.Dispose();
        }
    }
}
