using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
    public class SupplyItemRepository : Repository<Supplyitem>, ISupplyItemRepository
    {
        public SupplyItemRepository(MagazContext context) : base(context) { }

        
        public IEnumerable<Supplyitem> GetByProductId(int productId)
        {
            return _context.Supplyitems
                .Where(si => si.ProductId == productId)
                .ToList();

        }
        public Supplyitem GetByIdWithProduct(int id)
        {
            return _context.Supplyitems
                .Include(s => s.Product) 
                .FirstOrDefault(s => s.Id == id);
        }
    }
}