using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unison.Agent.Infrastructure.Data.Entities;

namespace Unison.Agent.Infrastructure.Data.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        void Add(Product product);

    }
}
