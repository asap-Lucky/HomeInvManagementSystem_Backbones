namespace HomeInvManagementAPI.DTOs.Request
{
    public class BatchUpdateLocationStockRequest
    {
        public required int ProductId { get; set; }

        public required int Delta { get; set; }
    }
}
