using Shop.Interfaces.DTO;

namespace Shop.Interfaces.Services
{
    public interface ILoyaltyService
    {
        LoyaltyCardDto GetCardByNumber(string cardNumber);
        void IssueNewCard(string cardNumber, string customerName);
        void UpdateCardLevel(int cardId); 
    }
}