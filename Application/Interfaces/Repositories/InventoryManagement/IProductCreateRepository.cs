using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.InventoryManagement
{
    public interface IProductCreateRepository
    {
        /// <summary>
        /// Adds a new product to the inventor using the provided CreateProductInDTO.
        /// </summary>
        /// <param name="createInDTO"></param>
        /// <returns></returns>
        public Task<CreateProductOutDTO> AddProductToInventoryDBAsync(CreateProductInDTO createInDTO);
    }
}
