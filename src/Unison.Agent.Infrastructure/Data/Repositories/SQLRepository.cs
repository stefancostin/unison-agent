﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Data;
using Unison.Agent.Core.Models;
using Unison.Agent.Infrastructure.Data.Adapters;

namespace Unison.Agent.Infrastructure.Data.Repositories
{
    public class SQLRepository : ISQLRepository
    {
        private readonly IDbContext _context;
        private readonly ILogger<SQLRepository> _logger;

        public SQLRepository(IDbContext context, ILogger<SQLRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public List<Dictionary<string, object>> Read(QuerySchema schema)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();

            using var connection = _context.GetConnection();
            try
            {
                connection.Open();

                var commandAdapter = new DbCommandAdapter(connection);
                using var command = commandAdapter.ConvertToDbCommand(schema);

                using var reader = command.ExecuteReader();
                var readerAdapter = new DbDataReaderAdapter(reader);
            
                while (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        result.Add(readerAdapter.Read());
                    }

                    reader.NextResult();
                }

                reader.Close();

                return result;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
