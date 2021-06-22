﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Data;
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

        public DataSet Read(QuerySchema schema)
        {
            DataSet result = new DataSet(schema.Entity, schema.PrimaryKey);

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
                        result.AddRecord(readerAdapter.Read());
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
