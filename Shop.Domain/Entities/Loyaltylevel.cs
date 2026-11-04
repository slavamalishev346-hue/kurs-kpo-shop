using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities;

public partial class Loyaltylevel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Percent { get; set; }

    public virtual ICollection<Loyaltycard> Loyaltycards { get; set; } = new List<Loyaltycard>();
}
