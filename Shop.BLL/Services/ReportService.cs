using System;
using Shop.Interfaces.DTO;
using Shop.Interfaces.Services;

namespace Shop.BLL.Services
{
    public class ReportService : IReportService
    {
        public SalesReportDto GetSalesReport(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public CashierReportDto GetCashierReport(int cashierId, DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}