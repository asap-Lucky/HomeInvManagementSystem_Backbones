using Domain.Enums;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace Application.DTOs.Outbound
{
    // Idea for a generic product DTO that will be mapped from various external product APIs.
    public class ProductAggregateDTO
    {
        public int ProductId { get; set; }

        public required string ProductName { get; set; }

        public string? EanCode { get; set; }

        public string? Brand { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? Category { get; set; }

        // ImageId used internally
        public int? ImageId { get; set; }

        // ImageUrl for external API use
        public string? ImageBLOB { get; set; }

        public List<ProductLocationDTO>? Locations { get; set; }

        public List<ProductCountryDTO>? CountriesOfOrigin { get; set; }

        public List<ProductSupplierDTO>? Suppliers { get; set; }

        public List<ProductTagDTO>? Tags { get; set; }
    }
}
