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

        public SaleDto CreateSale(List<SaleItemDto> items, string cardNumber, int cashierId)
        {
            Loyaltycard loyaltyCard = null;
            if (!string.IsNullOrWhiteSpace(cardNumber))
            {
                loyaltyCard = _unitOfWork.LoyaltyCards.GetList().FirstOrDefault(c => c.CardNumber == cardNumber);
                if (loyaltyCard == null)
                    throw new InvalidOperationException($"Карта с номером '{cardNumber}' не найдена.");
            }

            decimal totalAmount = 0;

            //  Сначала собираем все данные пока SupplyItem загружен
            var itemsData = new List<(int SupplyItemId, int Quantity, double PriceAtSale, string ProductName, string Article)>();

            foreach (var itemDto in items)
            {
                // ИСПРАВЛЕНО ЗДЕСЬ: используем GetByIdWithProduct, чтобы подгрузить Product
                var supplyItem = _unitOfWork.SupplyItems.GetByIdWithProduct(itemDto.SupplyItemId);

                if (supplyItem == null)
                    throw new InvalidOperationException($"Партия товара (ID: {itemDto.SupplyItemId}) не найдена.");

                if (supplyItem.QuantityRemain < itemDto.Quantity)
                    throw new InvalidOperationException($"Недостаточно товара '{supplyItem.Product.Name}'. Доступно: {supplyItem.QuantityRemain}");

                double priceAtSale = supplyItem.DiscountedPrice.HasValue ? supplyItem.DiscountedPrice.Value : supplyItem.Product.Price;

                itemsData.Add((
                    supplyItem.Id,
                    itemDto.Quantity,
                    priceAtSale,
                    supplyItem.Product.Name,
                    supplyItem.Product.Article
                ));

                totalAmount += (decimal)priceAtSale * itemDto.Quantity;
            }

            decimal discountAmount = 0;
            if (loyaltyCard != null && loyaltyCard.Level != null)
            {
                discountAmount = totalAmount * (decimal)(loyaltyCard.Level.Percent / 100.0);
            }

            decimal finalAmount = totalAmount - discountAmount;

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

            // Создаем SaleItem
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
                    Article = d.Article
                }).ToList()
            };
        }

        public SaleDto GetSaleById(int saleId)
        {
            var sale = _unitOfWork.Sales.GetById(saleId);
            if (sale == null) throw new InvalidOperationException("Продажа не найдена");
            return MapToDto(sale);
        }

        public List<SaleDto> GetAllSales()
        {
            return _unitOfWork.Sales.GetList().Select(MapToDto).ToList();
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
            if (sale == null) throw new InvalidOperationException("Оригинальная продажа не найдена.");

            foreach (var retDto in returnItems)
            {
                var originalSaleItem = sale.Saleitems.FirstOrDefault(si => si.Id == retDto.SaleItemId);
                if (originalSaleItem == null)
                    throw new InvalidOperationException("Товар не найден в оригинальном чеке.");

                if (originalSaleItem.Quantity < retDto.Quantity)
                    throw new InvalidOperationException($"Нельзя вернуть больше товара, чем было продано (Продано: {originalSaleItem.Quantity}, Возврат: {retDto.Quantity})");

                var supplyItem = _unitOfWork.SupplyItems.GetById(originalSaleItem.SupplyItemId);
                supplyItem.QuantityRemain += retDto.Quantity;
                _unitOfWork.SupplyItems.Update(supplyItem);

                var returnItem = new Returnitem
                {
                    SaleItemId = originalSaleItem.Id,
                    Quantity = retDto.Quantity,
                    CashierId = cashierId
                };

                _unitOfWork.ReturnItems.Add(returnItem);
            }

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
    }
}