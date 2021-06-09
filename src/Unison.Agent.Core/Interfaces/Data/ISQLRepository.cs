using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unison.Agent.Core.Interfaces.Data
{
    public interface ISQLRepository
    {
        List<Dictionary<string, object>> Execute(string sql);
    }
}
