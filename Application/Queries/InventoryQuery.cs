using Application.Interfaces.Queries;
using Domain.Enums;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;

namespace Application.Queries
{
    public class InventoryQuery : IInventoryQuery
    {
        // Injections
        private readonly IOpenFoodFactsQuery _openFoodFactsQuery;
        private readonly IProductReadRepository _prodReadRepo;

        public InventoryQuery(IOpenFoodFactsQuery openFoodFactsService, IProductReadRepository prodReadRepo)
        {
            _openFoodFactsQuery = openFoodFactsService;
            _prodReadRepo = prodReadRepo;
        }

        public async Task<ProductAggregateDTO> GetProductByEanAsync(string eanCode, ProductLocation location, SourceDestination source = SourceDestination.Auto)
        {
            try
            {
                switch (source)
                {
                    case SourceDestination.OFF:
                        var productFromOFF = await _openFoodFactsQuery.GetProductByEanAsync(eanCode);
                        return productFromOFF;

                    case SourceDestination.BTG:
                        Exception exception = new Exception("BTG not yet implemented.");
                        throw exception;

                    case SourceDestination.DB:
                        Exception exception1 = new Exception("DB not yet implemented.");
                        throw exception1;

                    default:
                        var productFromInventory = await _prodReadRepo.GetProductFromInventoryAsync(location, eanCode);
                        return productFromInventory;
                }
            }
            catch (Exception)
            {   
                throw;
            }
        }

        public async Task<List<ProductAggregateDTO>> GetProductsFromInventoryAsync(ProductLocation location, bool getAllLocations = false)
        {
            try
            {
                var productsInInventory = await _prodReadRepo.GetProductsFromInventoryAsync(location, getAllLocations);
                return productsInInventory;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
