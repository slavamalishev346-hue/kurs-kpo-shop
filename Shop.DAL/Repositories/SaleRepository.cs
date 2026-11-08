using System.Collections.Generic;
using System.Linq;
using Shop.DAL;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly MagazContext _context;

        public SaleRepository(MagazContext context)
        {
            _context = context;
        }

        public List<Sale> GetList()
        {
            return _context.Sales.ToList();
        }

        public Sale GetById(int id)
        {
            return _context.Sales.Find(id);
        }

        public void Add(Sale entity)
        {
            _context.Sales.Add(entity);
        }

        public void Update(Sale entity)
        {
            _context.Sales.Update(entity);
        }

        public void Delete(int id)
        {
            var sale = _context.Sales.Find(id);
            if (sale != null)
            {
                _context.Sales.Remove(sale);
            }
        }
    }
}