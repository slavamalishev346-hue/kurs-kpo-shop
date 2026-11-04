using Shop.Interfaces.DTO;
using System.Collections.Generic;

namespace Shop.Interfaces.Services
{
    public interface ISupplyService
    {
       
        SupplyDto CreateSupply(SupplyDto supply, List<SupplyItemDto> items);

      
        List<SupplyDto> GetAllSupplies();

      
        List<SupplyItemDto> GetAvailableStock();
    }
}