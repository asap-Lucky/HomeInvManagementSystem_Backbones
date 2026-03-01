using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductLocationDTO
    {
        public int LocationId { get; set; }

        public string LocationName { get; set; } = null!;

        public int Stock { get; set; }
    }
}
