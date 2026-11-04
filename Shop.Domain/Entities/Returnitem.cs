using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities;

public partial class Returnitem
{
    public int Id { get; set; }

    public int SaleItemId { get; set; }

    public int Quantity { get; set; }

    public int CashierId { get; set; }

    public virtual User Cashier { get; set; } = null!;

    public virtual Saleitem SaleItem { get; set; } = null!;
}
