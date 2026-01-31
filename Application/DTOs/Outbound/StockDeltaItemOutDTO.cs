using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Outbound
{
    public class StockDeltaItemOutDTO
    {
        public int ProductId { get; set; }
        public int Delta { get; set; }
        public int OpeningStock { get; set; }
        public int ClosingStock { get; set; }
    }
}
