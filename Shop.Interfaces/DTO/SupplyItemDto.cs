using System;

namespace Shop.Interfaces.DTO
{
    public class SupplyItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Cost { get; set; }           
        public DateTime? ExpiryDate { get; set; }   
        public int QuantityRemain { get; set; }     

      
        public string ProductName { get; set; }
        public string Article { get; set; }
    }
}