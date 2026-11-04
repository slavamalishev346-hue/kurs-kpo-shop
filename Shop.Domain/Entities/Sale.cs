using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities;

public partial class Sale
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public double TotalAmount { get; set; }

    public double? DiscountAmount { get; set; }

    public int CashierId { get; set; }

    public int? LoyaltyCardId { get; set; }

    public double FinalAmount { get; set; }

    public virtual User Cashier { get; set; } = null!;

    public virtual Loyaltycard? LoyaltyCard { get; set; }

    public virtual ICollection<Saleitem> Saleitems { get; set; } = new List<Saleitem>();
}
