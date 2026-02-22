using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.InventoryManagement
{
    public class ProductDeleteRepository : IProductDeleteRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<ProductDeleteRepository> _logger;

        public ProductDeleteRepository(HomeinvsystemContext context, ILogger<ProductDeleteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // NOTE: This does not delete the product as so but rather sets it to status "Deleted" so history can be saved of products bought in the future.
        public async Task<DeleteProductOutDTO> DeleteProductOnInventoryDBAsync(DeleteProductInDTO incomingDTO)
        {
            try
            {
                // 1. Change all locations stock for this product being on.
                // 2. Set the "IsDeleted" to true
                // 3. Set by default that products that are deleted are not being shown to the user. Set it in the Context file as to what GPT could define.

                // Comment: This should still be a delete since it does not neccesarily delete the ressource, but make it unaccessible 
                // for the user to access through normal API calls. Thats why step Number 3. Needs to be implemented so it works as such.

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error happened while trying to delete the product with id {incomingDTO.ProductId}");
                throw;
            }
        }
    }
}
