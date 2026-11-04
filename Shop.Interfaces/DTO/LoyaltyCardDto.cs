namespace Shop.Interfaces.DTO
{
    public class LoyaltyCardDto
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public int Points { get; set; }        
        public int Discount { get; set; }       
        public string Customer { get; set; }    
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public int LevelPercent { get; set; }  
    }
}