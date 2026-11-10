using System;
using System.Collections.Generic;
using System.Linq;
using Shop.Domain.Entities;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;
using Shop.Interfaces.UnitOfWork;

namespace Shop.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

      
        public List<ProductDto> GetAllProducts()
        {
            var products = _unitOfWork.Products.GetList();
            return products.Select(MapToDto).ToList();
        }

       
        public ProductDto? GetProductById(int id)
        {
            var product = _unitOfWork.Products.GetById(id);
            return product == null ? null : MapToDto(product);
        }

        //  Регистрация нового товара
        public void RegisterProduct(ProductDto dto)
        {
            // Проверка на уникальный артикул
            if (_unitOfWork.Products.GetByArticle(dto.Article) != null)
            {
                throw new InvalidOperationException($"Товар с артикулом '{dto.Article}' уже существует.");
            }

            var entity = new Product
            {
                Name = dto.Name,
                Article = dto.Article,
                Price = (double)dto.Price, 
                Cost = (double)dto.Cost
            };

            _unitOfWork.Products.Add(entity);
            _unitOfWork.SaveChanges();
        }

        public void UpdateProduct(ProductDto dto)
        {
            var entity = _unitOfWork.Products.GetById(dto.Id);
            if (entity == null) throw new InvalidOperationException("Товар не найден.");

            var existingByArticle = _unitOfWork.Products.GetByArticle(dto.Article);
            if (existingByArticle != null && existingByArticle.Id != dto.Id)
            {
                throw new InvalidOperationException($"Товар с артикулом '{dto.Article}' уже существует.");
            }

            entity.Name = dto.Name;
            entity.Article = dto.Article;
            entity.Price = (double)dto.Price;
            entity.Cost = (double)dto.Cost;

            _unitOfWork.Products.Update(entity);
            _unitOfWork.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = _unitOfWork.Products.GetById(id);
            if (product == null) throw new InvalidOperationException("Товар не найден.");

            // Считаем общий остаток, чтобы не удалить товар, который есть на складе
            int totalQty = product.Supplyitems?.Sum(si => si.QuantityRemain) ?? 0;
            if (totalQty > 0)
            {
                throw new InvalidOperationException("Нельзя удалить товар, у которого есть остатки на складе.");
            }

            _unitOfWork.Products.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void UpdatePricesByExpiration()
        {
            var supplyItems = _unitOfWork.SupplyItems.GetList();
            bool hasChanges = false;

            foreach (var item in supplyItems)
            {
                if (item.ExpiryDate.HasValue && item.QuantityRemain > 0)
                {
                    var supply = _unitOfWork.SupplyItems.GetById(item.Id)?.Supply;
                    

                    DateTime expiry = new DateTime(item.ExpiryDate.Value.Year, item.ExpiryDate.Value.Month, item.ExpiryDate.Value.Day);
                    if (expiry <= DateTime.Now.AddDays(30))
                    {
                        if (!item.DiscountedPrice.HasValue)
                        {
                            item.DiscountedPrice = item.Cost * 0.8; // Скидка 20%
                            _unitOfWork.SupplyItems.Update(item);
                            hasChanges = true;
                        }
                    }
                    else
                    {
                        if (item.DiscountedPrice.HasValue)
                        {
                            item.DiscountedPrice = null;
                            _unitOfWork.SupplyItems.Update(item);
                            hasChanges = true;
                        }
                    }
                }
            }

            if (hasChanges)
            {
                _unitOfWork.SaveChanges();
            }
        }

        public List<ProductDto> GetLowStockProducts()
        {
            return GetAllProducts().Where(p => p.TotalQuantity < 10).ToList();
        }

        public List<ProductDto> GetExpiringSoonProducts()
        {
            return GetAllProducts()
                .Where(p => p.NearestExpiryDate.HasValue && p.NearestExpiryDate.Value <= DateTime.Now.AddDays(20))
                .ToList();
        }

        private ProductDto MapToDto(Product entity)
        {
            var dto = new ProductDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Article = entity.Article,
                Price = (decimal)entity.Price,
                Cost = (decimal)entity.Cost
            };

            int totalQty = 0;
            DateTime? nearestExpiry = null;
            var supplyItemsDtos = new List<SupplyItemDto>();

            if (entity.Supplyitems != null)
            {
                foreach (var item in entity.Supplyitems)
                {
                    totalQty += item.QuantityRemain;

                   
                    if (item.QuantityRemain > 0 && item.ExpiryDate.HasValue)
                    {
                        DateTime itemExpiry = new DateTime(item.ExpiryDate.Value.Year, item.ExpiryDate.Value.Month, item.ExpiryDate.Value.Day);
                        if (!nearestExpiry.HasValue || itemExpiry < nearestExpiry.Value)
                        {
                            nearestExpiry = itemExpiry;
                        }
                    }

                  
                    supplyItemsDtos.Add(new SupplyItemDto
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        QuantityRemain = item.QuantityRemain,
                        Cost = (decimal)item.Cost,
                        DiscountedPrice = item.DiscountedPrice.HasValue ? (decimal)item.DiscountedPrice : null,
                        ExpiryDate = item.ExpiryDate.HasValue
                            ? new DateTime(item.ExpiryDate.Value.Year, item.ExpiryDate.Value.Month, item.ExpiryDate.Value.Day)
                            : null,
                        ProductName = entity.Name,
                        Article = entity.Article
                    });
                }
            }

            dto.TotalQuantity = totalQty;
            dto.NearestExpiryDate = nearestExpiry;
            dto.SupplyItems = supplyItemsDtos;

            return dto;
        }
    }
}