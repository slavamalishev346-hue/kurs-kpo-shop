using System.Collections.Generic;
using Shop.Domain.Entities;

namespace Shop.Interfaces.Repositories
{
  
    public interface IProductRepository : IRepository<Product>
    {
        
        Product? GetByArticle(string article);

        IEnumerable<Supplyitem> GetSupplyItemsByProductId(int productId);
    }
}