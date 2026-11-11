using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
   
    public class SaleRepository : Repository<Sale>, ISaleRepository
    {
        public SaleRepository(MagazContext context) : base(context) { }
    }
}