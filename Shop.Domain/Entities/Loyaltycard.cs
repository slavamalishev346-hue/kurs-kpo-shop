using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities;

public partial class Loyaltycard
{
    public int Id { get; set; }

    public string CardNumber { get; set; } = null!;

    public int? Points { get; set; }

    public int? Discount { get; set; }

    public string? Customer { get; set; }

    public int LevelId { get; set; }

    public virtual Loyaltylevel Level { get; set; } = null!;

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
