using HomeInvManagementAPI.Enums;

namespace Application.DTOs.Outbound
{
    // Idea for a generic product DTO that will be mapped from various external product APIs.
    public class ProductAggregateDTO
    {
        public string? EANCode { get; set; }

        public string? ProductName { get; set; }

        public string? Category { get; set; }

        public string? Brand { get; set; }

        public string? CountryOfOrigin { get; set; }

        public List<string>? Tags { get; set; }
    }
}
