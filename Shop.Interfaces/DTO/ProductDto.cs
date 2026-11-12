using System;
using System.Collections.Generic;

namespace Shop.Interfaces.DTO
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public string Article { get; set; } = null!;

        public int TotalQuantity { get; set; } 

       
        public DateTime? NearestExpiryDate { get; set; }

       
        public List<SupplyItemDto> SupplyItems { get; set; } = new();

        public decimal EffectivePrice
        {
            get
            {
                var firstAvailable = SupplyItems.FirstOrDefault(si => si.QuantityRemain > 0);
                if (firstAvailable != null && firstAvailable.DiscountedPrice.HasValue)
                    return firstAvailable.DiscountedPrice.Value;
                return Price;
            }
        }

        
        public bool HasDiscount => EffectivePrice < Price;
       
    }
}