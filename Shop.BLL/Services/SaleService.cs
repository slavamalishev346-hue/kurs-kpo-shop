using System;
using System.Collections.Generic;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;

namespace Shop.BLL.Services
{
    public class SaleService : ISaleService
    {
        public List<SaleDto> GetSales()
        {
            throw new NotImplementedException();
        }

        public SaleDto GetSaleById(int saleId)
        {
            throw new NotImplementedException();
        }

        public SaleDto CreateSale(int cashierId, string loyaltyCardNumber, List<SaleItemDto> items)
        {
            throw new NotImplementedException();
        }

        public void ProcessReturn(int saleId, List<ReturnDto> returnItems, int cashierId)
        {
            throw new NotImplementedException();
        }
    }
}