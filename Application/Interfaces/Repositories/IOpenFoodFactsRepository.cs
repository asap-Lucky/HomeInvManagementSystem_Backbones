using Application.DTOs.Outbound.API;

namespace HomeInvManagementAPI.Interfaces.Repositories
{
    public interface IOpenFoodFactsRepository
    {
        public Task<OpenFoodFactsResponseApiDto>? GetByEanAsync(string ean);
    }
}
