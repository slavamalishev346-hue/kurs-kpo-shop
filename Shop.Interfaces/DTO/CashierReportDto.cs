using System;

namespace Shop.Interfaces.DTO
{
    public class CashierReportDto
    {
        public string CashierName { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal TotalAmount { get; set; }      
        public int SalesCount { get; set; }            
        public int ReturnsCount { get; set; }          
        public decimal AveragePerDay { get; set; }     
    }
}