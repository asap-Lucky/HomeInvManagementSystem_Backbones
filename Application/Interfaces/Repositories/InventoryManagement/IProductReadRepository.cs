using Application.DTOs.Outbound;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.InventoryManagement
{
    public interface IProductReadRepository
    {
        /// <summary>
        /// Gets a product from the inventory based on location and optional EAN code.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="eanCode"></param>
        /// <returns></returns>
        public Task<ProductAggregateDTO> GetProductFromInventoryAsync(ProductLocation location, string? eanCode = null);

        /// <summary>
        /// Gets a list of products from the inventory based on location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Task<List<ProductAggregateDTO>> GetProductsFromInventoryAsync(ProductLocation location, bool getAllLocations = false);
    }
}
