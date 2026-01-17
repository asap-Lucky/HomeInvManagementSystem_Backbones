namespace Application.DTOs.Outbound
{
    public class CreateProductOutDTO
    {
        public int ProductId { get; set; }

        public required string ProductName { get; set; }

        public string? EanCode { get; set; }

        public string? Brand { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? Category { get; set; }

        public int? ImageId { get; set; }

        public List<ProductLocationDTO>? Locations { get; set; }

        public List<ProductCountryDTO>? CountriesOfOrigin { get; set; }

        public List<ProductSupplierDTO>? Suppliers { get; set; }

        public List<ProductTagDTO>? Tags { get; set; }
    }
}
