using Application.DTOs.Inbound;
using Application.DTOs.Outbound;

namespace Infrastructure.Repositories.InventoryManagement
{
    public interface IProductDeleteRepository
    {
        Task<DeleteProductOutDTO> DeleteProductOnInventoryDBAsync(DeleteProductInDTO incomingDTO);
    }
}