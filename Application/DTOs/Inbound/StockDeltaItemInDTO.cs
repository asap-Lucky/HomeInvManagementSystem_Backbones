using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inbound
{
    public class StockDeltaItemInDTO
    {
        public int ProductId { get; set; }
        public int Delta { get; set; }
    }
}
