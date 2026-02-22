using System;
using System.Collections.Generic;

namespace Infrastructure.Models.HomeInv;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Barcode { get; set; }

    public string? Brand { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int? ImageId { get; set; }

    public int CategoryId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Image? Image { get; set; }

    public virtual ICollection<ProductLocation> ProductLocations { get; set; } = new List<ProductLocation>();

    public virtual ICollection<Country> Countries { get; set; } = new List<Country>();

    public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
