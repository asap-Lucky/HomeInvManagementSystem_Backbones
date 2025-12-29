using Application.DTOs;

namespace HomeInvManagementAPI.DTOs.Response
{
    public class CreateProductResponse
    {
        public int ProductId { get; set; }

        public required string ProductName { get; set; }

        public string? EanCode { get; set; }

        public string? Brand { get; set; }
        
        public DateTime? ExpirationDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? Category { get; set; }

        public string? ImageBLOB { get; set; }

        public List<string>? Locations { get; set; }

        // NOTE: One product can be produced in multiple countries. Example: Nuts collected in Bolivia, packaged for Lidl in Germany.
        public List<string>? CountriesOfOrigin { get; set; }

        public List<string>? Suppliers { get; set; }

        public List<string>? Tags { get; set; }
    }
}
