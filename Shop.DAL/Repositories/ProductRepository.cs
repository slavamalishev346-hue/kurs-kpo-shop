using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shop.DAL.Repositories;
using Shop.Domain.Entities;
using Shop.Interfaces.Repositories;

namespace Shop.DAL.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(MagazContext context) : base(context) { }

        public Product? GetByArticle(string article)
        {
            return _context.Products.FirstOrDefault(p => p.Article == article);
        }

        public IEnumerable<Supplyitem> GetSupplyItemsByProductId(int productId)
        {
            return _context.Supplyitems
                .Where(si => si.ProductId == productId)
                .ToList();
        }

        public new List<Product> GetList()
        {
            var products = _context.Products.ToList();

            // Для каждого товара подгружаем его партии вручную
            foreach (var product in products)
            {
                product.Supplyitems = _context.Supplyitems
                    .Where(si => si.ProductId == product.Id)
                    .ToList();
            }

            return products;
        }

        public new Product? GetById(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                product.Supplyitems = _context.Supplyitems
                    .Where(si => si.ProductId == product.Id)
                    .ToList();
            }
            return product;
        }
    }
}