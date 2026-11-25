using System;
using Shop.Interfaces.Repositories; 

namespace Shop.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ISaleRepository Sales { get; }
        ISaleItemRepository SaleItems { get; }
        ISupplyRepository Supplies { get; }
        ISupplyItemRepository SupplyItems { get; }
        ILoyaltyCardRepository LoyaltyCards { get; }
        ILoyaltyLevelRepository LoyaltyLevels { get; }
        IUserRepository Users { get; }
        IReturnItemRepository ReturnItems { get; }

        int SaveChanges();
    }
}