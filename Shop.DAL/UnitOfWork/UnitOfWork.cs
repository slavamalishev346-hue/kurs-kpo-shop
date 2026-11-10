using System;
using Shop.DAL.Repositories;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;
using Shop.Interfaces.UnitOfWork;

namespace Shop.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MagazContext _context;

        private IUserRepository _users;
        private ILoyaltyCardRepository _loyaltyCards;
        private ILoyaltyLevelRepository _loyaltyLevels;
        private ISaleRepository _sales;
        private IProductRepository? _products;
        private ISupplyItemRepository? _supplyItems;
        public UnitOfWork(MagazContext context)
        {
            _context = context;
        }

        
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public ILoyaltyCardRepository LoyaltyCards => _loyaltyCards ??= new LoyaltyCardRepository(_context);

        public ILoyaltyLevelRepository LoyaltyLevels => _loyaltyLevels ??= new LoyaltyLevelRepository(_context);
        public ISaleRepository Sales
        {
            get
            {
                if (_sales == null)
                {
                    _sales = new SaleRepository(_context);
                }
                return _sales;
            }
        }
        public IProductRepository Products
        {
            get
            {
                if (_products == null)
                    _products = new ProductRepository(_context);
                return _products;
            }
        }

        public ISupplyItemRepository SupplyItems
        {
            get
            {
                if (_supplyItems == null)
                    _supplyItems = new SupplyItemRepository(_context);
                return _supplyItems;
            }
        }
     
        public ISaleItemRepository SaleItems => null;
        public ISupplyRepository Supplies => null;
    

        public IReturnItemRepository ReturnItems => null;

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}