using Application.DTOs.Outbound;

namespace HomeInvManagementAPI.Mappers
{
    public static class OpenFoodFactsMapper
    {
        public static ProductAggregateDTO MapToProductAggregateDTO(OpenFoodFactsResponseApiDto openFoodFactsResponse)
        {
            var product = openFoodFactsResponse.Product;

            if (product == null)
                throw new ArgumentNullException(nameof(product), "Product data is null in the OpenFoodFacts response.");

            return new ProductAggregateDTO
            {
                EANCode = openFoodFactsResponse.EanCode ?? string.Empty,
                ProductName = product.ProductName ?? "Unknown Product",
                Brand = product.Brand ?? "Unknown Brand",
                CountriesOfOrigin = product.Countries ?? new List<string>(),
                Tags = product.Keywords ?? new List<string>()
            };
        }
    }
}
