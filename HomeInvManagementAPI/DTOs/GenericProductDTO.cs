using HomeInvManagementAPI.Enums;

namespace HomeInvManagementAPI.DTOs
{
    public class GenericProductDTO
    {
        public string EANCode { get; set; }

        public string ProductName { get; set; }

        public ProductCategory Category { get; set; }
    }
}
