using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
  
    public class ReturnItemRepository : Repository<Returnitem>, IReturnItemRepository
    {
        public ReturnItemRepository(MagazContext context) : base(context) { }
    }
}