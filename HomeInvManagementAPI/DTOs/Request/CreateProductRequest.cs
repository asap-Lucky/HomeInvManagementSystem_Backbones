
using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace HomeInvManagementAPI.DTOs.Request
{
    public class CreateProductRequest
    {
        [Required]
        public string ProductName { get; set; }
        public ProductCategory Category { get; set; }
        public List<ProductLocation> Locations { get; set; }
        public string? EANCode { get; set; }
        public string? Brand { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<string>? CountriesOfOrigin { get; set; }
        public List<string>? Suppliers { get; set; }
        public string? ImageBLOB { get; set; }  
        public List<string>? Tags { get; set; }
    }
}