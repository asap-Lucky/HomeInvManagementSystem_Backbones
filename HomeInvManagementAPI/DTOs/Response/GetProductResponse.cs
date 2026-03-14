using Application.DTOs;

namespace HomeInvManagementAPI.DTOs.Response
{
    public class GetProductResponse
    {
        public string? ProductId { get; set; }

        public string? ProductName { get; set; }

        public string? Barcode { get; set; }

        public string? Brand { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ImageId { get; set; }

        public bool IsDeleted { get; set; }

        public ProductCategoryDTO? Category { get; set; }

        public List<ProductLocationDTO>? Locations { get; set; }

        public List<ProductCountryDTO>? CountriesOfOrigin { get; set; }

        public List<ProductSupplierDTO>? Suppliers { get; set; }

        public List<ProductTagDTO>? Tags { get; set; }
    }
}
