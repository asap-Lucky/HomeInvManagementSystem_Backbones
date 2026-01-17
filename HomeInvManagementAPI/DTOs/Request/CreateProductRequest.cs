
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HomeInvManagementAPI.DTOs.Request
{
    public class CreateProductRequest
    {
        [Required]
        public required string ProductName { get; set; }
        public ProductCategory Category { get; set; }
        public List<ProductLocation> Locations { get; set; } = null!;
        public string? EANCode { get; set; }
        public string? Brand { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<int>? OriginCountries { get; set; }
        public List<int>? Suppliers { get; set; }
        public int? ImageId { get; set; }  
        public List<int>? Tags { get; set; }
    }
}