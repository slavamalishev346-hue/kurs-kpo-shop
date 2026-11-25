using System;
using System.Collections.Generic;
using System.Linq;
using Shop.Domain.Entities;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;
using Shop.Interfaces.UnitOfWork;

namespace Shop.BLL.Services
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public (decimal Total, decimal Discount, decimal Final) CalculateTotals(List<SaleItemDto> items, decimal discountPercent)
        {
            decimal total = items.Sum(i => i.PriceAtSale * i.Quantity);
          
            decimal discountable = items.Where(i => !i.IsDiscounted).Sum(i => i.PriceAtSale * i.Quantity);
            decimal discount = discountable * discountPercent / 100m;
            return (total, discount, total - discount);
        }
        public SaleDto CreateSale(List<SaleItemDto> items, string cardNumber, int cashierId)
        {
            Loyaltycard loyaltyCard = null;
            if (!string.IsNullOrWhiteSpace(cardNumber))
            {
                loyaltyCard = _unitOfWork.LoyaltyCards.GetList().FirstOrDefault(c => c.CardNumber == cardNumber);
                if (loyaltyCard == null)
                    throw new InvalidOperationException($"Карта с номером '{cardNumber}' не найдена.");
            }

            var itemsData = new List<(int SupplyItemId, int Quantity, double PriceAtSale, string ProductName, string Article, bool IsDiscounted)>();

            foreach (var itemDto in items)
            {
                var supplyItem = _unitOfWork.SupplyItems.GetByIdWithProduct(itemDto.SupplyItemId);

                if (supplyItem == null)
                    throw new InvalidOperationException($"Партия товара (ID: {itemDto.SupplyItemId}) не найдена.");

                if (supplyItem.QuantityRemain < itemDto.Quantity)
                    throw new InvalidOperationException($"Недостаточно товара '{supplyItem.Product.Name}'. Доступно: {supplyItem.QuantityRemain}");

                if (supplyItem.ExpiryDate.HasValue && supplyItem.ExpiryDate.Value < DateOnly.FromDateTime(DateTime.Today))
                    throw new InvalidOperationException($"Товар '{supplyItem.Product.Name}' просрочен — продажа запрещена.");

                double priceAtSale = supplyItem.DiscountedPrice.HasValue ? supplyItem.DiscountedPrice.Value : supplyItem.Product.Price;

                itemsData.Add((
                    supplyItem.Id,
                    itemDto.Quantity,
                    priceAtSale,
                    supplyItem.Product.Name,
                    supplyItem.Product.Article,
                    supplyItem.DiscountedPrice.HasValue
                ));
            }

            decimal percent = loyaltyCard?.Level != null ? (decimal)loyaltyCard.Level.Percent : 0m;
            var (totalAmount, discountAmount, finalAmount) = CalculateTotals(
                itemsData.Select(d => new SaleItemDto
                {
                    Quantity = d.Quantity,
                    PriceAtSale = (decimal)d.PriceAtSale,
                    IsDiscounted = d.IsDiscounted
                }).ToList(),
                percent);

            var sale = new Sale
            {
                Date = DateTime.Now,
                TotalAmount = (double)totalAmount,
                DiscountAmount = (double)discountAmount,
                FinalAmount = (double)finalAmount,
                CashierId = cashierId,
                LoyaltyCardId = loyaltyCard?.Id
            };

            _unitOfWork.Sales.Add(sale);
            _unitOfWork.SaveChanges();

            foreach (var data in itemsData)
            {
                var newSaleItem = new Saleitem
                {
                    SaleId = sale.Id,
                    SupplyItemId = data.SupplyItemId,
                    Quantity = data.Quantity,
                    PriceAtSale = data.PriceAtSale
                };

                _unitOfWork.SaleItems.Add(newSaleItem);

                var supplyItem = _unitOfWork.SupplyItems.GetById(data.SupplyItemId);
                supplyItem.QuantityRemain -= data.Quantity;
                _unitOfWork.SupplyItems.Update(supplyItem);
            }

            _unitOfWork.SaveChanges();

            if (loyaltyCard != null)
            {
                loyaltyCard.Points += (int)(finalAmount * 0.01m);
                _unitOfWork.LoyaltyCards.Update(loyaltyCard);
                CheckAndUpgradeLoyaltyLevel(loyaltyCard);
                _unitOfWork.SaveChanges();
            }

            return new SaleDto
            {
                Id = sale.Id,
                Date = sale.Date,
                TotalAmount = totalAmount,
                DiscountAmount = discountAmount,
                FinalAmount = finalAmount,
                CashierName = sale.Cashier?.FullName,
                CardNumber = loyaltyCard?.CardNumber,
                Items = itemsData.Select(d => new SaleItemDto
                {
                    SupplyItemId = d.SupplyItemId,
                    Quantity = d.Quantity,
                    PriceAtSale = (decimal)d.PriceAtSale,
                    ProductName = d.ProductName,
                    Article = d.Article,
                    IsDiscounted = d.IsDiscounted
                }).ToList()
            };
        }

        public SaleDto GetSaleById(int saleId)
        {
            var sale = _unitOfWork.Sales.GetById(saleId);
            if (sale == null) throw new InvalidOperationException("Продажа не найдена");

            var dto = MapToDto(sale);

            var saleItems = _unitOfWork.SaleItems.GetBySaleId(saleId).ToList();
            var returns = _unitOfWork.ReturnItems.GetList().ToList();

            dto.Items = saleItems.Select(si =>
            {
                var supplyItem = _unitOfWork.SupplyItems.GetByIdWithProduct(si.SupplyItemId);

                return new SaleItemDto
                {
                    Id = si.Id,
                    SupplyItemId = si.SupplyItemId,
                    Quantity = si.Quantity,
                    PriceAtSale = (decimal)si.PriceAtSale,
                    ProductName = supplyItem?.Product?.Name,
                    Article = supplyItem?.Product?.Article,
                    AlreadyReturned = returns.Where(r => r.SaleItemId == si.Id).Sum(r => r.Quantity)
                };
            }).ToList();

            return dto;
        }

        public List<SaleDto> GetAllSales()
        {
            var sales = _unitOfWork.Sales.GetList().Select(MapToDto).ToList();

            var saleItems = _unitOfWork.SaleItems.GetList().ToList();
            var returns = _unitOfWork.ReturnItems.GetList().ToList();

            var soldBySale = saleItems
                .GroupBy(si => si.SaleId)
                .ToDictionary(g => g.Key, g => g.Sum(si => si.Quantity));

            var returnedBySale = saleItems
                .Join(returns, si => si.Id, r => r.SaleItemId, (si, r) => new { si.SaleId, r.Quantity })
                .GroupBy(x => x.SaleId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            foreach (var dto in sales)
            {
                soldBySale.TryGetValue(dto.Id, out var sold);
                returnedBySale.TryGetValue(dto.Id, out var returned);
                dto.IsFullyReturned = sold > 0 && returned >= sold;
            }

            return sales;
        }

        public List<SaleDto> GetSalesByDateRange(DateTime startDate, DateTime endDate)
        {
            return _unitOfWork.Sales.GetList()
                .Where(s => s.Date >= startDate && s.Date <= endDate)
                .Select(MapToDto).ToList();
        }

        public void ProcessReturn(int saleId, List<ReturnDto> returnItems, int cashierId)
        {
            var sale = _unitOfWork.Sales.GetById(saleId);
            if (sale == null)
                throw new InvalidOperationException("Оригинальная продажа не найдена.");

            // Грузим строки чека и возвраты по ним отдельно
            var originalItems = _unitOfWork.SaleItems.GetBySaleId(saleId).ToList();
            var saleItemIds = originalItems.Select(i => i.Id).ToList();
            var relevantReturns = _unitOfWork.ReturnItems.GetList()
                .Where(r => saleItemIds.Contains(r.SaleItemId)).ToList();

            bool hasItemsToReturn = false;

            foreach (var retDto in returnItems)
            {
                if (retDto.Quantity <= 0) continue;
                hasItemsToReturn = true;

                var originalSaleItem = originalItems.FirstOrDefault(si => si.Id == retDto.SaleItemId);
                if (originalSaleItem == null)
                    throw new InvalidOperationException("Товар не найден в оригинальном чеке.");

                // Защита от повторного возврата
                int alreadyReturned = relevantReturns.Where(r => r.SaleItemId == originalSaleItem.Id).Sum(r => r.Quantity);
                int availableToReturn = originalSaleItem.Quantity - alreadyReturned;

                if (retDto.Quantity > availableToReturn)
                    throw new InvalidOperationException($"Нельзя вернуть {retDto.Quantity} шт. Доступно только {availableToReturn} шт.");

                // Возвращаем остаток в партию
                var supplyItem = _unitOfWork.SupplyItems.GetById(originalSaleItem.SupplyItemId);
                supplyItem.QuantityRemain += retDto.Quantity;
                _unitOfWork.SupplyItems.Update(supplyItem);

                // Пишем факт возврата в БД с датой и суммой
                var returnItem = new Returnitem
                {
                    SaleItemId = originalSaleItem.Id,
                    Quantity = retDto.Quantity,
                    CashierId = cashierId,
                    Date = DateTime.Now,
                    Amount = retDto.Quantity * originalSaleItem.PriceAtSale
                };

                _unitOfWork.ReturnItems.Add(returnItem);
            }

            if (!hasItemsToReturn)
                throw new InvalidOperationException("Нет товаров для возврата (количество равно 0).");

            _unitOfWork.SaveChanges();
        }
        private void CheckAndUpgradeLoyaltyLevel(Loyaltycard card)
        {
            var levels = _unitOfWork.LoyaltyLevels.GetList().OrderBy(l => l.Percent).ToList();
            var currentLevel = levels.FirstOrDefault(l => l.Id == card.LevelId);
            if (currentLevel == null) return;

            foreach (var level in levels)
            {
                if (level.Id > currentLevel.Id && card.Points >= level.Percent * 1000)
                {
                    card.LevelId = level.Id;
                    card.Discount = (int)level.Percent;
                    _unitOfWork.LoyaltyCards.Update(card);
                    break;
                }
            }
        }

        private SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                Date = sale.Date,
                TotalAmount = (decimal)sale.TotalAmount,
                DiscountAmount = sale.DiscountAmount.HasValue ? (decimal)sale.DiscountAmount : 0,
                FinalAmount = (decimal)sale.FinalAmount,
                CashierName = sale.Cashier?.FullName,
                CardNumber = sale.LoyaltyCard?.CardNumber,
                Items = sale.Saleitems?.Select(si => new SaleItemDto
                {
                    Id = si.Id,
                    SupplyItemId = si.SupplyItemId,
                    Quantity = si.Quantity,
                    PriceAtSale = (decimal)si.PriceAtSale,
                    ProductName = si.SupplyItem?.Product?.Name,
                    Article = si.SupplyItem?.Product?.Article
                }).ToList() ?? new List<SaleItemDto>()
            };
        }
        public decimal GetLoyaltyDiscountPercent(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return 0;

            var card = _unitOfWork.LoyaltyCards.GetByCardNumber(cardNumber);

            if (card == null)
                throw new InvalidOperationException($"Карта '{cardNumber}' не найдена.");

            return card.Level != null ? (decimal)card.Level.Percent : 0;
        }
        public decimal CalculateRefund(List<SaleItemDto> items)
        {
            return items.Sum(i => i.PriceAtSale * i.ReturnQuantity);
        }
    }
}