using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities;

public partial class Supply
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string SupplierName { get; set; } = null!;

    public int Count { get; set; }

    public virtual ICollection<Supplyitem> Supplyitems { get; set; } = new List<Supplyitem>();
}
