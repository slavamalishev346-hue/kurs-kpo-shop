using System;
using Shop.DAL.Repositories;
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
        private IProductRepository _products;
        private ISupplyItemRepository _supplyItems;

        // ✅ Добавляем поля для недостающих репозиториев
        private ISaleItemRepository _saleItems;
        private ISupplyRepository _supplies;
        private IReturnItemRepository _returnItems;

        public UnitOfWork(MagazContext context)
        {
            _context = context;
        }

        public IUserRepository Users => _users ??= new UserRepository(_context);
        public ILoyaltyCardRepository LoyaltyCards => _loyaltyCards ??= new LoyaltyCardRepository(_context);
        public ILoyaltyLevelRepository LoyaltyLevels => _loyaltyLevels ??= new LoyaltyLevelRepository(_context);

        public ISaleRepository Sales
        {
            get => _sales ??= new SaleRepository(_context);
        }

        public IProductRepository Products
        {
            get => _products ??= new ProductRepository(_context);
        }

        public ISupplyItemRepository SupplyItems
        {
            get => _supplyItems ??= new SupplyItemRepository(_context);
        }

        // ✅ ИСПРАВЛЕННЫЕ СВОЙСТВА (больше никаких null!)
        public ISaleItemRepository SaleItems
        {
            get => _saleItems ??= new SaleItemRepository(_context);
        }

        public ISupplyRepository Supplies
        {
            get => _supplies ??= new SupplyRepository(_context);
        }

        public IReturnItemRepository ReturnItems
        {
            get => _returnItems ??= new ReturnItemRepository(_context);
        }

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