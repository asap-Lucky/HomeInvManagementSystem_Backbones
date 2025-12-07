using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class InventoryCommand : IInventoryCommand
    {
        // Injections
        private readonly ILogger<InventoryCommand> _logger;
        private readonly IInventoryManagementRepository _inventoryManagementRepository;

        public InventoryCommand(ILogger<InventoryCommand> logger, IInventoryManagementRepository inventoryManagementRepository)
        {
            _logger = logger;
            _inventoryManagementRepository = inventoryManagementRepository;
        }

        public async Task<CreateProductOutDTO> AddProductToInventoryAsync(CreateProductInDTO createProdInDTO)
        {
            try
            {
                var createProduct = await _inventoryManagementRepository.AddProductToInventoryAsync(createProdInDTO);
                return createProduct;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
