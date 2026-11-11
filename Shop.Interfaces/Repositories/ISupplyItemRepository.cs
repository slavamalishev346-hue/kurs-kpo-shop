using System.Collections.Generic;
using Shop.Domain.Entities;

namespace Shop.Interfaces.Repositories
{

    public interface ISupplyItemRepository : IRepository<Supplyitem>
    {
       
        IEnumerable<Supplyitem> GetByProductId(int productId);
        Supplyitem GetByIdWithProduct(int id);

    }
}