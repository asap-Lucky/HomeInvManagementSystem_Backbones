using System;
using System.Collections.Generic;

namespace Infrastructure.Models.HomeInv;

public partial class Tag
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
