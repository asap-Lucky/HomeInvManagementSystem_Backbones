using Domain.Entities;
using Domain.Enums;

namespace HomeInvManagementAPI.DTOs.Request
{
    public class CreateProductRequest
    {
        public required string ProductName { get; set; }
        public required ProductCategory Category { get; set; }
        public required List<ProductLocation> Locations { get; set; }
        public string? EANCode { get; set; }
        public string? Brand { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<string>? CountriesOfOrigin { get; set; }
        public List<string>? Suppliers { get; set; }
        public string? ImageBLOB { get; set; }  
        public List<string>? Tags { get; set; }
    }
}