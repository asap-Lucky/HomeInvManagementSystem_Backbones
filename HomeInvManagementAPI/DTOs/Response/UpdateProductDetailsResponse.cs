using Application.DTOs;

namespace HomeInvManagementAPI.DTOs.Response
{
    /// <summary>
    /// NOTE: Locations are not included in the UpdateProductDetailsResponse as they are updated separately through the UpdateProductLocations endpoint.
    /// </summary>
  
    public class UpdateProductDetailsResponse
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? Barcode { get; set; }

        public string? Brand { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? ImageId { get; set; }

        public ProductCategoryDTO Category { get; set; } = null!;

        public List<ProductCountryDTO>? OriginCountries { get; set; }

        public List<ProductSupplierDTO>? Suppliers { get; set; }

        public List<ProductTagDTO>? Tags { get; set; }
    }
}
