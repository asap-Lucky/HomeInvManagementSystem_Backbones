using System;
using System.Collections.Generic;

namespace Infrastructure.Models.HomeInv;

public partial class Itemtype
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public virtual Category Category { get; set; } = null!;
}
