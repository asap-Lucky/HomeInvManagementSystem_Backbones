using Application.Interfaces.Queries;
using Microsoft.Extensions.Logging;
using Domain.Enums;

using Application.DTOs.Outbound;
using System.Reflection.Metadata.Ecma335;

namespace Application.Queries
{
    public class ProductAggregateQuery : IProductAggregateQuery
    {
        // Injections
        private readonly ILogger<ProductAggregateQuery> _logger;
        private readonly IOpenFoodFactsQuery _openFoodFactsQuery;

        public ProductAggregateQuery(ILogger<ProductAggregateQuery> logger, IOpenFoodFactsQuery openFoodFactsService)
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
