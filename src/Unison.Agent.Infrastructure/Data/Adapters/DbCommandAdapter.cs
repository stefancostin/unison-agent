using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Models;

namespace Unison.Agent.Infrastructure.Data.Adapters
{
    public class DbCommandAdapter
    {
        private const string FieldSeparator = ", ";
        private const string ParameterIdentifier = "@";

        public DbCommandAdapter(DbConnection connection)
        {
            Connection = connection;
        }

        public DbConnection Connection { get; set; }

        public DbCommand ConvertToDbCommand(QuerySchema schema)
        {
            var sanitizedSchema = SanitizeIdentifiers(schema);

            var sql = GenerateSql(sanitizedSchema);
            var command = CreateDbCommand(sanitizedSchema, sql);

            return command;
        }

        private DbCommand CreateDbCommand(QuerySchema schema, string sql)
        {
            var command = new SqlCommand(sql, (SqlConnection)Connection);
            command.CommandType = CommandType.Text;

            if (schema.Conditions.Any())
                SanitizeAndAddParams(schema, command);

            return command;
        }

        private string CreateParameter(string field) => ParameterIdentifier + field;

        private string GenerateSql(QuerySchema schema)
        {
            string selectStatement = GenerateSelectStatement(schema);

            if (!schema.Conditions.Any())
                return selectStatement;

            string whereSatement = GenerateWhereStatement(schema);

            return $"{selectStatement} {whereSatement}";
        }

        private static string GenerateSelectStatement(QuerySchema schema)
        {
            var table = schema.Entity;
            var fields = string.Join(FieldSeparator, schema.Fields);

            return $"{Sql.Select} {fields} {Sql.From} {table}";
        }

        private string GenerateWhereStatement(QuerySchema schema)
        {
            var conditionsCount = schema.Conditions.Count();
            var conditionFields = schema.Conditions.Select(p => p.Name).ToList();
            var conditionParams = schema.Conditions.Select(p => CreateParameter(p.Name)).ToList();

            var conditions = new StringBuilder($"{Sql.Where} ");
            for (int i = 0; i < conditionsCount; i++)
            {
                conditions.Append($"{conditionFields[i]} = {conditionParams[i]}");

                if (i < conditionsCount - 1)
                    conditions.Append($" {Sql.And} ");
            }

            return $"{Sql.Where} {conditions}";
        }

        private void SanitizeAndAddParams(QuerySchema schema, SqlCommand command)
        {
            foreach (var param in schema.Conditions)
            {
                command.Parameters.AddWithValue(CreateParameter(param.Name), param.Value);
            }
        }

        private QuerySchema SanitizeIdentifiers(QuerySchema schema)
        {
            var commandBuilder = new SqlCommandBuilder();
            var sanitizedSchema = new QuerySchema();
            sanitizedSchema.Entity = commandBuilder.QuoteIdentifier(schema.Entity);
            sanitizedSchema.Fields = schema.Fields.Select(field => commandBuilder.QuoteIdentifier(field));
            sanitizedSchema.Conditions = schema.Conditions.Select(param => new QueryParam()
            {
                Name = commandBuilder.QuoteIdentifier(param.Name),
                Value = param.Value,
            });
            return sanitizedSchema;
        }
    }

    internal static class Sql
    {
        public const string Select = "SELECT";
        public const string Distinct = "DISTINCT";
        public const string From = "FROM";
        public const string Where = "WHERE";
        public const string OrderBy = "ORDER BY";
        public const string GroupBy = "GROUP BY";
        public const string Having = "HAVING";
        public const string And = "AND";
        public const string Or = "OR";
    }
}
