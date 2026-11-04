using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities;

public partial class Supplyitem
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public double Cost { get; set; }

    public int SupplyId { get; set; }

    public int ProductId { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public int QuantityRemain { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Saleitem> Saleitems { get; set; } = new List<Saleitem>();

    public virtual Supply Supply { get; set; } = null!;
}
