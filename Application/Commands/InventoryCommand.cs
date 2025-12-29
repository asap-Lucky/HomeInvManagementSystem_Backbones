using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.InventoryManagement;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class InventoryCommand : IInventoryCommand
    {
        // Injections
        private readonly IProductCreateRepository _prodCreateRepo;

        public InventoryCommand(IProductCreateRepository productCreate)
        {
            _prodCreateRepo = productCreate;
        }

        public async Task<CreateProductOutDTO> AddProductToInventoryAsync(CreateProductInDTO createProdInDTO)
        {
            try
            {
                var createProduct = await _prodCreateRepo.AddProductToInventoryAsync(createProdInDTO);
                return createProduct;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
