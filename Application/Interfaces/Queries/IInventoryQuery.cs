using Application.DTOs.Outbound;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Queries
{
    public interface IInventoryQuery
    {
        Task<GetProductOutDTO> GetProductById(string id);



        public Task<List<ProductAggregateDTO>> GetProductsFromInventoryAsync(ProductLocation location, bool getAllLocations = false);
    }
}
