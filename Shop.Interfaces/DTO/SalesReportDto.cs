using System;

namespace Shop.Interfaces.DTO
{
    public class SalesReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }    
        public int TotalSales { get; set; }         
        public decimal AverageCheck { get; set; }     
    }
}