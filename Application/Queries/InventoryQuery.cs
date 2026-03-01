using Application.Interfaces.Queries;
using Domain.Enums;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;
using Application.DTOs;
using System.Linq.Expressions;

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

        public async Task<GetProductOutDTO> GetProductById(string id)
        {
            try
            {
                var outDTO = await _prodReadRepo.GetProductByIdAsync(id);
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

        public async Task<bool> IsLocationExisting(int locationId)
        {
            try
            {
                ProductLocationDTO location = await _prodReadRepo.GetLocationByIdAsync(locationId);

                if (location == null || location == new ProductLocationDTO())
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
