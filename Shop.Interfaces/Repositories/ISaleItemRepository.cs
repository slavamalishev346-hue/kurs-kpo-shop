using System.Collections.Generic;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories; 

namespace Shop.Interfaces.Repositories
{
   
    public interface ISaleItemRepository : IRepository<Saleitem>
    {
        IEnumerable<Saleitem> GetBySaleId(int saleId);
    }
}