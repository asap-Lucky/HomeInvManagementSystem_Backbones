using System;
using System.Collections.Generic;

namespace Infrastructure.Models.HomeInv;

public partial class ProductLocation
{
    public int ProductId { get; set; }

    public int LocationId { get; set; }

    public int Quantity { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
