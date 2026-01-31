using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inbound
{
    public class BatchUpdateLocationStockInDTO
    {
        public int LocationId { get; set; }

        public List<StockDeltaItemInDTO> StockDeltas { get; set; } = new();
    }
}
