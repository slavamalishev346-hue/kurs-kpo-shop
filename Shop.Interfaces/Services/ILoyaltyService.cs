using Shop.Interfaces.DTO;

namespace Shop.Interfaces.Services
{
    public interface ILoyaltyService
    {
  
        LoyaltyCardDto GetCardByNumber(string cardNumber);

        // повышение уровня карты 
        void UpdateCardLevel(int cardId);
    }
}