using Shop.Interfaces.DTO;
using System;
using System.Collections.Generic;

namespace Shop.Interfaces.Services
{
    public interface ISaleService
    {
       
        List<SaleDto> GetSales();

        
        SaleDto GetSaleById(int saleId);

        // Функция 2, 3, 4, 5 из ТЗ: 
  
        SaleDto CreateSale(int cashierId, string loyaltyCardNumber, List<SaleItemDto> items);

        // Возврат
        void ProcessReturn(int saleId, List<ReturnDto> returnItems, int cashierId);
    }
}