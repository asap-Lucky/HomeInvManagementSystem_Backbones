using Application.DTOs.Outbound;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Queries
{
    public interface IProductAggregateQuery
    {
        public Task<ProductAggregateDTO> GetProductByEanAsync(string ean, SourceDestinations source);
    }
}
