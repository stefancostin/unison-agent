using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Workers;
using Unison.Agent.Core.Services;
using Unison.Agent.Core.Services.Workers;

namespace Unison.Agent.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<ITimedWorker, HeartbeatWorker>();
            services.AddScoped<ISubscriptionWorker, SyncWorker>();

            services.AddHostedService<SubscriberManager>();
            //services.AddHostedService<TimedServiceManager>();

        }
    }
}
