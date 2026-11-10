using System;

namespace Shop.Interfaces.DTO
{
    public class SupplyItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int QuantityRemain { get; set; }
        public decimal Cost { get; set; }

        //  Цена со скидкой.
        public decimal? DiscountedPrice { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public string ProductName { get; set; } = null!;
        public string Article { get; set; } = null!;
    }
}