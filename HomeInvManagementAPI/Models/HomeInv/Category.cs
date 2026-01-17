using System;
using System.Collections.Generic;

namespace HomeInvManagementAPI.Models.HomeInv;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Itemtype> Itemtypes { get; set; } = new List<Itemtype>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
