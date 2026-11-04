using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities;

public partial class Saleitem
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public double PriceAtSale { get; set; }

    public int SaleId { get; set; }

    public int SupplyItemId { get; set; }

    public virtual ICollection<Returnitem> Returnitems { get; set; } = new List<Returnitem>();

    public virtual Sale Sale { get; set; } = null!;

    public virtual Supplyitem SupplyItem { get; set; } = null!;
}
