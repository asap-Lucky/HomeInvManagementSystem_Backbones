namespace Application.DTOs.Inbound
{
    public class CreateProductInDTO
    {
        public required string ProductName { get; set; }
        public int Category { get; set; }
        public string? EANCode { get; set; }
        public string? Brand { get; set; }
        public string? ImageBLOB { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<string>? CountriesOfOrigin { get; set; }    
        public List<string>? Suppliers { get; set; }
        public List<int>? Locations { get; set; }
        public List<string>? Tags { get; set; }
    }
}
