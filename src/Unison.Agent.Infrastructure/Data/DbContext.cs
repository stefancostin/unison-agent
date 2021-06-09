using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Data;

namespace Unison.Agent.Infrastructure.Data
{
    /// <summary>
    /// Retrieves a db connection from the connection pool that ADO.NET creates when provided the same connection string
    /// </summary>
    public class DbContext : IDbContext
    {
        private readonly IConfiguration _config;

        public DbContext(IConfiguration config)
        {
            _config = config;
        }

        /// <returns>Returns a connection from the connection pool</returns>
        public DbConnection GetConnection()
        {
            return new SqlConnection(_config.GetConnectionString("Unison"));
        }
    }
}
