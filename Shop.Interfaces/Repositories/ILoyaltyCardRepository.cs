using System.Collections.Generic;
using Shop.Domain.Entities;

namespace Shop.Interfaces.Repositories
{
    public interface ILoyaltyCardRepository : IRepository<Loyaltycard>
    {
        Loyaltycard GetByCardNumber(string cardNumber);
    }
}