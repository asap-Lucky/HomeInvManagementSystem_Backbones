using System;
using System.Collections.Generic;

namespace Infrastructure.Models.HomeInv;

public partial class Image
{
    public int Id { get; set; }

    public byte[] Data { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
