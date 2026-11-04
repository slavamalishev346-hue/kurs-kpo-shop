using System;
using System.Collections.Generic;

namespace Shop.Interfaces.DTO
{
    public class SaleDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }

      
        public string CashierName { get; set; }

 
        public int? LoyaltyCardId { get; set; }
        public string CardNumber { get; set; }


        public List<SaleItemDto> Items { get; set; } = new List<SaleItemDto>();
    }
}