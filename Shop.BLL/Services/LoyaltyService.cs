using System;
using System.Linq;
using Shop.Interfaces.Services;
using Shop.Interfaces.UnitOfWork;
using Shop.Interfaces.DTO;
using Shop.Domain.Entities;

namespace Shop.BLL.Services
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoyaltyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public LoyaltyCardDto GetCardByNumber(string cardNumber)
        {
            var entity = _unitOfWork.LoyaltyCards.GetByCardNumber(cardNumber);

            if (entity == null)
            {
                return null;
            }

            var dto = new LoyaltyCardDto();
            dto.Id = entity.Id;
            dto.CardNumber = entity.CardNumber;
            dto.Points = entity.Points ?? 0;
            dto.Discount = entity.Discount ?? 0;
            dto.Customer = entity.Customer;
            dto.LevelId = entity.LevelId;

            if (entity.Level != null)
            {
                dto.LevelName = entity.Level.Name;
                dto.LevelPercent = (int)entity.Level.Percent;
            }
            else
            {
                dto.LevelName = "Неизвестно";
                dto.LevelPercent = 0;
            }

            return dto;
        }

        public void IssueNewCard(string cardNumber, string customerName)
        {
            var existingCard = _unitOfWork.LoyaltyCards.GetByCardNumber(cardNumber);

            if (existingCard != null)
            {
                throw new Exception("Карта с таким номером уже существует!");
            }

            var newCard = new Loyaltycard();
            newCard.CardNumber = cardNumber;
            newCard.Customer = customerName;
            newCard.Points = 0;
            newCard.Discount = 0;
            newCard.LevelId = 1;

            _unitOfWork.LoyaltyCards.Add(newCard);
            _unitOfWork.SaveChanges();
        }

        // === МЕТОД ПОВЫШЕНИЯ УРОВНЯ ПО СУММЕ ПРОДАЖ ===
        public void UpdateCardLevel(int cardId)
        {
            // 1. Получаем карту
            var card = _unitOfWork.LoyaltyCards.GetById(cardId);

            if (card == null)
            {
                throw new Exception("Карта не найдена!");
            }

            // 2. Считаем общую сумму продаж по этой карте
            // Получаем все продажи и фильтруем по LoyaltyCardId
            var allSales = _unitOfWork.Sales.GetList();

            var totalSpent = allSales
                .Where(s => s.LoyaltyCardId == cardId)
                .Sum(s => s.TotalAmount);

            // 3. Определяем новый уровень на основе суммы
            int newLevelId;

            if (totalSpent >= 50000)
            {
                newLevelId = 3; // Золотой (10%)
            }
            else if (totalSpent >= 10000)
            {
                newLevelId = 2; // Серебряный (5%)
            }
            else
            {
                newLevelId = 1; // Базовый (0%)
            }

            // 4. Если уровень изменился — обновляем
            if (card.LevelId != newLevelId)
            {
                card.LevelId = newLevelId;
                _unitOfWork.LoyaltyCards.Update(card);
                _unitOfWork.SaveChanges();
            }
        }
    }
}