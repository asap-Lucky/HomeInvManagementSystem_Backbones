using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class InventoryCommand : IInventoryCommand
    {
        // Injections
        private readonly ILogger<InventoryCommand> _logger;

        public InventoryCommand(ILogger<InventoryCommand> logger)
        {
            _logger = logger;
        }

        public async Task<CreateProductInDTO> AddProductToInventoryAsync(CreateProductOutDTO createOutDTO)
        {
            try
            {


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
