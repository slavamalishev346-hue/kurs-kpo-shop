using System;
using System.Collections.Generic;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;

namespace Shop.BLL.Services
{
    public class SupplyService : ISupplyService
    {
        public SupplyDto CreateSupply(SupplyDto supply, List<SupplyItemDto> items)
        {
            throw new NotImplementedException();
        }

        public List<SupplyDto> GetAllSupplies()
        {
            throw new NotImplementedException();
        }

        public List<SupplyItemDto> GetAvailableStock()
        {
            throw new NotImplementedException();
        }
    }
}