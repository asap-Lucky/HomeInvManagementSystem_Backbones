using HomeInvManagementAPI.DTOs.OpenFoodFactsProduct;
using HomeInvManagementAPI.DTOs.OpenFoodFactsProduct.Outbound;

namespace HomeInvManagementAPI.Interfaces.OpenFoodFacts
{
    public interface IOpenFoodFactsService
    {
        public Task<OpenFoodFactsResponseApiDto>? GetProductByEanCodeAsync(string eanCode);
    }
}
