using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Data;
using Unison.Agent.Infrastructure.Amqp;
using Unison.Agent.Infrastructure.Data;
using Unison.Agent.Infrastructure.Data.Repositories;
using Unison.Common.Amqp;
using Unison.Common.Amqp.Interfaces;

namespace Unison.Agent.Infrastructure
{
    public static class StartupSetup
    {
        public static void AddDbContext(this IServiceCollection services)
        {
            services.AddSingleton<IDbContext, DbContext>();
            services.AddScoped<ISQLRepository, SQLRepository>();

            //services.AddDbContext<AppDbContext>();
            //services.AddScoped<IProductRepository, ProductRepository>();
        }

        public static void AddAmqpContext(this IServiceCollection services)
        {
            services.AddAmqpInfrastructure();

            services.AddScoped<IAmqpInfrastructureInitializer, AmqpInfrastructureInitializer>();
            services.AddScoped<IAmqpSubscriberInitializer, AmqpSubscriberInitializer>();
        }
    }
}
