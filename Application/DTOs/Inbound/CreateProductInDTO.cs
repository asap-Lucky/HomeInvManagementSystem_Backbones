namespace Application.DTOs.Inbound
{
    public class CreateProductInDTO
    {
        public required string ProductName { get; set; }
        public int Category { get; set; }
        public string? EANCode { get; set; }
        public string? Brand { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ImageId { get; set; }
        public List<int>? OriginCountries { get; set; }    
        public List<int>? Suppliers { get; set; }
        public List<int>? Locations { get; set; }
        public List<int>? Tags { get; set; }
    }
}
