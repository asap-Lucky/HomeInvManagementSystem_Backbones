using Domain.Enums;
using System.Reflection.Metadata;

namespace Application.DTOs.Outbound
{
    // Idea for a generic product DTO that will be mapped from various external product APIs.
    public class ProductAggregateDTO
    {
        public string? ProductName { get; set; }

        public string? Category { get; set; }

        public string? EANCode { get; set; }

        public string? Brand { get; set; }

        public List<string>? CountriesOfOrigin { get; set; }

        public string? ImageBLOB { get; set; }

        public List<ProductLocation> Locations { get; set; }

        public List<string> Suppliers { get; set; }

        public List<string>? Tags { get; set; }
    }
}
