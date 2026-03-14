using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Outbound
{
    public class GetProductLocationOutDTO
    {
        public string? ProductId { get; set; }

        public string? ProductName { get; set; }

        public int Stock { get; set; }
    }
}
