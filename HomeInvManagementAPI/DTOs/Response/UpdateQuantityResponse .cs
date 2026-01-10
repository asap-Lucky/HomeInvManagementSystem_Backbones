using Domain.Enums;

namespace HomeInvManagementAPI.DTOs.Response
{
    public class ProductQuantityResponse
    {
        public ProductLocation LocationId { get; set; }

        public ProductLocation LocationName { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
