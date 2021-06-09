using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Interfaces.Data;

namespace Unison.Agent.Infrastructure.Data.Services
{
    public class DbDataReaderAdapter
    {
        private readonly DbDataReader _reader;
        private List<string> _columns;

        public DbDataReaderAdapter(DbDataReader reader)
        {
            _reader = reader;
        }

        public Dictionary<string, object> Read()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            foreach (var column in GetColumns())
            {
                result.Add(column, _reader[column]);
            }

            return result;
        }

        private List<string> ExtractColumns()
        {
            List<string> columns = new List<string>();

            DataTable schemaTable = _reader.GetSchemaTable();

            foreach (DataRow row in schemaTable.Rows)
            {
                foreach (DataColumn column in schemaTable.Columns)
                {
                    columns.Add((string)row[column]);
                    break;
                }
            }

            return columns;
        }


        private List<string> GetColumns()
        {
            if (_columns == null)
                _columns = ExtractColumns();

            return _columns;
        }
    }
}
