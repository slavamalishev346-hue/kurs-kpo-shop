namespace Shop.Interfaces.DTO
{
    public class SaleItemDto
    {
        public int Id { get; set; }
        public int SupplyItemId { get; set; } 
        public int Quantity { get; set; }
        public decimal PriceAtSale { get; set; } 

        public string ProductName { get; set; }
        public string Article { get; set; }
    }
}