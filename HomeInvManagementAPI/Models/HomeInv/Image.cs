using System;
using System.Collections.Generic;

namespace HomeInvManagementAPI.Models.HomeInv;

public partial class Image
{
    public int Id { get; set; }

    public byte[] Data { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Extension { get; set; } = null!;

    public bool IsAssigned { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
