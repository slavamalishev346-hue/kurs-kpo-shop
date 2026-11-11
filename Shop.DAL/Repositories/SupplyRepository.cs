using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
   
    public class SupplyRepository : Repository<Supply>, ISupplyRepository
    {
       
        public SupplyRepository(MagazContext context) : base(context) { }
    }
}