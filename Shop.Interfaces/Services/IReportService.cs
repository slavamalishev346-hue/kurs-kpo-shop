using Shop.Interfaces.DTO;
using System;

namespace Shop.Interfaces.Services
{
    public interface IReportService
    {
        //  Отчет по продажам за период
        SalesReportDto GetSalesReport(DateTime startDate, DateTime endDate);

        // Отчет по кассиру 
        CashierReportDto GetCashierReport(int cashierId, DateTime date);
    }
}