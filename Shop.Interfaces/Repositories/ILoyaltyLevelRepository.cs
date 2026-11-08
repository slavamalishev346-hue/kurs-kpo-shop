using System.Collections.Generic;
using Shop.Domain.Entities;

namespace Shop.Interfaces.Repositories
{
    public interface ILoyaltyLevelRepository : IRepository<Loyaltylevel>
    {
        List<Loyaltylevel> GetAll();
    }
}