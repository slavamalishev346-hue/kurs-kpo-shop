using System;
using System.Collections.Generic;

namespace Shop.Domain.Entities;

public partial class Report
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Data { get; set; }

    public int AdminId { get; set; }

    public virtual User Admin { get; set; } = null!;
}
