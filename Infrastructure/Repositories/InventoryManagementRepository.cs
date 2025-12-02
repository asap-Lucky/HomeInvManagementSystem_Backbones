using Application.DTOs.Outbound;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class InventoryManagementRepository : IInventoryManagementRepository
    {
        // Injections
        private readonly ILogger<InventoryManagementRepository> _logger;

        public InventoryManagementRepository(ILogger<InventoryManagementRepository> logger)
        {
            _logger = logger;
        }

        public async Task<CreateProductInDTO> AddProductToInventoryAsync(CreateProductInDTO createInDTO)
        {
            try
            {
                // TODO: Imp base response wrapper later to set some values that are needed later.
                //BaseRequestWrapper<CreateProductRequest> requestWrapper = new()
                //{
                //};

                // Implementation for adding product to inventory goes here.
                throw new NotImplementedException("AddProductToInventoryAsync is not yet implemented.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CreateProductInDTO>> AddProductsBulkToInventoryAsync(List<CreateProductInDTO> createBulkInDTO)
        {
            try
            {
                // TODO: Imp base response wrapper later to set some values that are needed later.
                // Implementation for adding product to inventory goes here.
                throw new NotImplementedException("AddProductToInventoryAsync is not yet implemented.");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
