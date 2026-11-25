using System;
using Shop.Domain.Entities;

namespace Shop.Domain.Entities;

public partial class Returnitem
{
    public int Id { get; set; }

    public int SaleItemId { get; set; }

    public int Quantity { get; set; }

    public int CashierId { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    public double Amount { get; set; }

    public virtual User Cashier { get; set; } = null!;

    public virtual Saleitem SaleItem { get; set; } = null!;
}