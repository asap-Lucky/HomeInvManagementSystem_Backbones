using Application.Commands;
using HomeInvManagementAPI.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Application.Interfaces.Queries;
using Application.DTOs.Outbound;
using HomeInvManagementAPI.Mappers;
using Application.DTOs.Outbound.API;

namespace Application.Queries
{
    public class OpenFoodFactsQuery : IOpenFoodFactsQuery
    {
        // Injections
        private readonly ILogger<OpenFoodFactsQuery> _logger;
        private readonly IOpenFoodFactsRepository _openFoodFactsRepository;

        public OpenFoodFactsQuery(ILogger<OpenFoodFactsQuery> logger, IOpenFoodFactsRepository openFoodFactsRepository)
        {
            _logger = logger;
            _openFoodFactsRepository = openFoodFactsRepository;
        }
    
        public async Task<OFFOutDTO>? GetProductByEanAsync(string eanCode)
        {
            try
            {
                OpenFoodFactsResponseApiDto productResponse = await _openFoodFactsRepository.GetByEanAsync(eanCode);

                OFFOutDTO mappedProduct = OpenFoodFactsMapper.MapToProductAggregateDTO(productResponse);

                return mappedProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product details from OpenFoodFacts.");
                throw;
            }
        }
    }
}
