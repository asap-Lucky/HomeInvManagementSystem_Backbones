using Application.DTOs;
using Domain.Enums;

namespace HomeInvManagementAPI.DTOs.Response
{
    public class UpdateProductDetailsResponse
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? EanCode { get; set; }

        public int? ImageId { get; set; }

        public string? Brand { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ProductCategoryDTO Category { get; set; } = null!;

        public List<ProductCountryDTO>? OriginCountries { get; set; }

        public List<ProductSupplierDTO>? Suppliers { get; set; }

        public List<ProductTagDTO>? Tags { get; set; }
    }
}
