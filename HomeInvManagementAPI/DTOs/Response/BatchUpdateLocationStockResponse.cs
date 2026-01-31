using Application.DTOs;
using Application.DTOs.Outbound;

namespace HomeInvManagementAPI.DTOs.Response
{
    public class BatchUpdateLocationStockResponse
    {
        public ProductLocationDTO Location { get; set; } = null!;

        public List<StockDeltaItemOutDTO> UpdatedStocks { get; set; } = null!;

        public string? TransactionId { get; set; }
    }
}
