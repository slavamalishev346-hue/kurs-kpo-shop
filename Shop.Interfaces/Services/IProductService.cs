using System.Collections.Generic;
using Shop.Interfaces.DTO;

namespace Shop.Interfaces.Services
{
    public interface IProductService
    {
        void RegisterProduct(ProductDto product);
        List<ProductDto> GetAllProducts();

        void UpdatePricesByExpiration();

        ProductDto? GetProductById(int id);
        void UpdateProduct(ProductDto dto);
        void DeleteProduct(int id);

        List<ProductDto> GetLowStockProducts();
        List<ProductDto> GetExpiringSoonProducts();
    }
}