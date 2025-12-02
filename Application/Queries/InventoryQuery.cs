using Application.Interfaces.Queries;
using Microsoft.Extensions.Logging;
using Domain.Enums;
using System.Reflection.Metadata.Ecma335;
using Application.DTOs.Outbound;

namespace Application.Queries
{
    public class InventoryQuery : IInventoryQuery
    {
        // Injections
        private readonly ILogger<InventoryQuery> _logger;
        private readonly IOpenFoodFactsQuery _openFoodFactsQuery;

        public InventoryQuery(ILogger<InventoryQuery> logger, IOpenFoodFactsQuery openFoodFactsService)
        {
            _logger = logger;
            _openFoodFactsQuery = openFoodFactsService;
        }

        public async Task<ProductAggregateDTO> GetProductByEanAsync(string ean, SourceDestination source, ProductLocation location)
        {
            try
            {
                if (source == SourceDestination.OFF)
                {
                    var productFromOFF = await _openFoodFactsQuery.GetProductByEanAsync(ean);
                    return productFromOFF;
                }

                if (source == SourceDestination.BTG)
                {
                    Exception exception = new Exception("BTG not yet implemented.");
                    throw exception;
                }

                if (source == SourceDestination.Auto)
                {
                    Exception exception = new Exception("Auto not yet implemented.");
                    throw exception;
                }

                if (source == SourceDestination.DB)
                {
                    Exception exception = new Exception("DB not yet implemented.");
                    throw exception;
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
