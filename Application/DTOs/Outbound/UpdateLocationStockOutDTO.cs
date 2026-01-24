using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Outbound
{
    public class UpdateLocationStockOutDTO
    {
        public int ProductId { get; set; }
        public ProductLocationDTO Location { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
