using Application.Interfaces.Queries;
using Microsoft.Extensions.Logging;
using Domain.Enums;
using System.Reflection.Metadata.Ecma335;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories;

namespace Application.Queries
{
    public class InventoryQuery : IInventoryQuery
    {
        // Injections
        private readonly ILogger<InventoryQuery> _logger;
        private readonly IOpenFoodFactsQuery _openFoodFactsQuery;
        private readonly IInventoryManagementRepository _inventoryManagementRepository;


        public InventoryQuery(ILogger<InventoryQuery> logger, IOpenFoodFactsQuery openFoodFactsService, IInventoryManagementRepository inventoryManagementRepo)
        {
            _logger = logger;
            _openFoodFactsQuery = openFoodFactsService;
            _inventoryManagementRepository = inventoryManagementRepo;
        }

        public async Task<ProductAggregateDTO> GetProductByEanAsync(string ean, ProductLocation location, SourceDestination source = SourceDestination.Auto)
        {
            try
            {
                switch (source)
                {
                    case SourceDestination.OFF:
                        var productFromOFF = await _openFoodFactsQuery.GetProductByEanAsync(ean);
                        return productFromOFF;

                    case SourceDestination.BTG:
                        Exception exception = new Exception("BTG not yet implemented.");
                        throw exception;

                    case SourceDestination.DB:
                        Exception exception1 = new Exception("DB not yet implemented.");
                        throw exception1;

                    default:
                        var productFromInventory = await _inventoryManagementRepository.GetProductFromInventoryAsync(location, ean);
                        return productFromInventory;
                }
            }
            catch (Exception)
            {   
                throw;
            }
        }

        public async Task<List<ProductAggregateDTO>> GetProductsFromInventoryAsync(ProductLocation location)
        {
            try
            {
                var productsInInventory = await _inventoryManagementRepository.GetProductsFromInventoryAsync(location);
                return productsInInventory;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
