using Application.DTOs.Outbound;

namespace HomeInvManagementAPI.Interfaces.Repositories
{
    public interface IOpenFoodFactsRepository
    {
        public Task<OpenFoodFactsResponseApiDto>? GetByEanAsync(string ean);
    }
}
