
using Domain.Enums;
using Infrastructure.Models.HomeInv;
using System.ComponentModel.DataAnnotations;

namespace HomeInvManagementAPI.DTOs.Request
{
    /// <summary>
    /// The model for adding a new product to the inventory to later be able to edit their ammount in the inventory. 
    /// Notes:
    /// - 'ProductName' must be added
    /// - 'CategoryId' must be added
    /// - Location must be defined at the very least - If not on a specific location, set it to location id "Unassigned".
    /// - 'OriginCountries' are a list due to the product being able to originate/be produced in multiple countries.
    /// </summary>
    
    public class CreateProductRequest
    {
        [Required]
        public required string ProductName { get; set; }
        [Required]
        public required int CategoryId { get; set; }
        [Required]
        [MinLength(1)]
        public required List<int> Locations { get; set; }       
        public string? Barcode { get; set; }
        public string? Brand { get; set; }
        public int? ImageId { get; set; }
        public List<int>? OriginCountries { get; set; }
        public List<int>? Suppliers { get; set; }
        public List<int>? Tags { get; set; }
    }
}