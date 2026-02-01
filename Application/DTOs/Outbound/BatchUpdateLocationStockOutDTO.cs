using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Outbound
{
    public class BatchUpdateLocationStockOutDTO
    {
        public ProductLocationDTO Location { get; set; } = null!;

        public List<StockDeltaItemOutDTO>? StockDeltas { get; set; } = new();

        public string? TransactionId { get; set; }

        public DateTime TransactionTimeStamp => DateTime.Now;
    }
}
