using System;
using System.Collections.Generic;

namespace HomeInvManagementAPI.Models.HomeInv;

public partial class Location
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductLocation> ProductLocations { get; set; } = new List<ProductLocation>();
}
