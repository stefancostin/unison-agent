using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Configuration;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Agent.Core.Models;
using Unison.Agent.Core.Services;
using Unison.Agent.Core.Services.Workers;
using Unison.Common.Amqp.DTO;

namespace Unison.Agent.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IAgentConfiguration>((serviceProvider) =>
                configuration.GetSection("Agent").Get<AgentConfiguration>());

            services.AddSingleton<IAmqpConfiguration>((serviceProvider) =>
                configuration.GetSection("Amqp").Get<AmqpConfiguration>());
        }

        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ITimedWorker, HeartbeatWorker>();
            services.AddScoped<ISubscriptionWorker<AmqpMessage>, SyncWorker>();

            services.AddHostedService<SubscriberManager>();
            //services.AddHostedService<TimedServiceManager>();
        }

    }
}
