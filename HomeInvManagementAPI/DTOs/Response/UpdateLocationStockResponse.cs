using Application.DTOs;
using Domain.Enums;

namespace HomeInvManagementAPI.DTOs.Response
{
    public class UpdateLocationStockResponse
    {
        public int ProductId { get; set; }
        public ProductLocationDTO Location { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
