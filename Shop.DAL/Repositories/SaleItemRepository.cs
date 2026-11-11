using System.Collections.Generic;
using System.Linq;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
    
    public class SaleItemRepository : Repository<Saleitem>, ISaleItemRepository
    {
        public SaleItemRepository(MagazContext context) : base(context) { }

        public IEnumerable<Saleitem> GetBySaleId(int saleId)
        {
            return _context.Saleitems.Where(si => si.SaleId == saleId).ToList();
        }
    }
}