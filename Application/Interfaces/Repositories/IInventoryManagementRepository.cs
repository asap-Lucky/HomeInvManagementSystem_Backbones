using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IInventoryManagementRepository
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
        public Task<List<ProductAggregateDTO>> GetProductsFromInventoryAsync(ProductLocation location);

        /// <summary>
        /// Adds a new product to the inventor using the provided CreateProductInDTO.
        /// </summary>
        /// <param name="createInDTO"></param>
        /// <returns></returns>
        public Task<CreateProductOutDTO> AddProductToInventoryAsync(CreateProductInDTO createInDTO);

        /// <summary>
        /// Creates multiple products in bulk in the inventory using the provided list of CreateProductInDTO. 
        /// </summary>
        /// <param name="createBulkInDTO"></param>
        /// <returns></returns>
        public Task<List<CreateProductOutDTO>> AddProductsBulkToInventoryAsync(List<CreateProductInDTO> createBulkInDTO);
    }
}