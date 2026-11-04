using System;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;

namespace Shop.BLL.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        public LoyaltyCardDto GetCardByNumber(string cardNumber)
        {
            throw new NotImplementedException();
        }

        public void UpdateCardLevel(int cardId)
        {
            throw new NotImplementedException();
        }
    }
}