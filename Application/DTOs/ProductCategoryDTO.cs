using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductCategoryDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
