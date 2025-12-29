using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.InventoryManagement
{
    public class ProductUpdateRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;

        public ProductUpdateRepository(HomeinvsystemContext context)
        {
            _context = context;
        }
    }
}
