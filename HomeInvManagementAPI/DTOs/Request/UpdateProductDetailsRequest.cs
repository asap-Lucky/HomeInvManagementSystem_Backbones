using Application.DTOs;

namespace HomeInvManagementAPI.DTOs.Request
{
    public class UpdateProductDetailsRequest
    {
        public string ProductName { get; set; } = null!;

        public string? EanCode { get; set; }

        public string? Brand { get; set; }

        public int CategoryId { get; set; }

        public int? ImageId { get; set; }

        public List<int>? OriginCountries { get; set; }

        public List<int>? Suppliers { get; set; }

        public List<int>? Tags { get; set; }
    }
}
