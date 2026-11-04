using System;
using System.Collections.Generic;

namespace Shop.Interfaces.DTO
{
    public class SupplyDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string SupplierName { get; set; }
        public int Count { get; set; } 

       
        public List<SupplyItemDto> Items { get; set; } = new List<SupplyItemDto>();
    }
}