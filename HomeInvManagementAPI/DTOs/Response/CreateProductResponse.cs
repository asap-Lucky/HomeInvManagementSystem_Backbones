using Application.DTOs;

namespace HomeInvManagementAPI.DTOs.Response
{
    public class CreateProductResponse
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? Barcode { get; set; }

        public string? Brand { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? ImageId { get; set; }

        public ProductCategoryDTO Category { get; set; } = null!;

        public List<ProductLocationDTO>? Locations { get; set; }

        public List<ProductCountryDTO>? OriginCountries { get; set; }

        public List<ProductSupplierDTO>? Suppliers { get; set; }

        public List<ProductTagDTO>? Tags { get; set; }
    }
}
