using System;
using System.Collections.Generic;
using Shop.Interfaces.DTO;

namespace Shop.Interfaces.Services
{
    public interface ISaleService
    {
        SaleDto GetSaleById(int saleId);
        List<SaleDto> GetAllSales();
        List<SaleDto> GetSalesByDateRange(DateTime startDate, DateTime endDate);

        SaleDto CreateSale(List<SaleItemDto> items, string cardNumber, int cashierId);

        decimal GetLoyaltyDiscountPercent(string cardNumber);
        void ProcessReturn(int saleId, List<ReturnDto> returnItems, int cashierId);
    }
}