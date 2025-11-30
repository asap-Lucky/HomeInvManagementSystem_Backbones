using Application.DTOs.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Queries
{
    public interface IOpenFoodFactsQuery
    {
        public Task<ProductAggregateDTO>? GetProductByEanAsync(string eanCode);
    }
}
