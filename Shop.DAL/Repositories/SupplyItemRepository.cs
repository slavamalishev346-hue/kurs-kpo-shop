using System.Collections.Generic;
using System.Linq;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
    public class SupplyItemRepository : Repository<Supplyitem>, ISupplyItemRepository
    {
        public SupplyItemRepository(MagazContext context) : base(context) { }

        // Получить все партии товара
        public IEnumerable<Supplyitem> GetByProductId(int productId)
        {
            return _context.Supplyitems
                .Where(si => si.ProductId == productId)
                .ToList();
        }
    }
}