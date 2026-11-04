using Shop.Interfaces.DTO;
using System.Collections.Generic;

namespace Shop.Interfaces.Services
{
    public interface IProductService
    {
       
        void RegisterProduct(ProductDto product);

        List<ProductDto> GetAllProducts();

        //Автоматическое изменение цен по сроку годности
        void UpdatePricesByExpiration();
    }
}