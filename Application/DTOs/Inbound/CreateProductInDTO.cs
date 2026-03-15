using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Inbound
{
    public class CreateProductInDTO
    {
        public required string ProductName { get; set; }

        public string? Barcode { get; set; }

        public string? Brand { get; set; }

        public int CategoryId { get; set; }

        public int? ImageId { get; set; }

        public List<int>? OriginCountries { get; set; }    

        public List<int>? Suppliers { get; set; }

        public required List<int> Locations { get; set; }

        public List<int>? Tags { get; set; }
    }
}
