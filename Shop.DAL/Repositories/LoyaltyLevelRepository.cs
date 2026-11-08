using System.Collections.Generic;
using System.Linq;
using Shop.DAL;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
    public class LoyaltyLevelRepository : ILoyaltyLevelRepository
    {
        private readonly MagazContext _context;

        public LoyaltyLevelRepository(MagazContext context)
        {
            _context = context;
        }

        public List<Loyaltylevel> GetAll()
        {
            return _context.Loyaltylevels.ToList();
        }

        public List<Loyaltylevel> GetList()
        {
            return _context.Loyaltylevels.ToList();
        }

        public Loyaltylevel GetById(int id)
        {
            return _context.Loyaltylevels.Find(id);
        }

        public void Add(Loyaltylevel entity)
        {
            _context.Loyaltylevels.Add(entity);
        }

        public void Update(Loyaltylevel entity)
        {
            _context.Loyaltylevels.Update(entity);
        }

        public void Delete(int id)
        {
            var level = _context.Loyaltylevels.Find(id);

            if (level != null)
            {
                _context.Loyaltylevels.Remove(level);
            }
        }
    }
}