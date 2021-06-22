using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Core.Data;
using Unison.Agent.Core.Models;

namespace Unison.Agent.Core.Interfaces.Data
{
    public interface ISQLRepository
    {
        DataSet Read(QuerySchema schema);
    }
}
