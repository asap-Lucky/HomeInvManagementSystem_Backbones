using Application.DTOs.Inbound;
using Application.DTOs.Outbound;

namespace Application.Interfaces.Repositories.InventoryManagement
{
    public interface IProductDeleteRepository
    {
        Task<DeleteProductOutDTO> DeleteProductOnInventoryDBAsync(DeleteProductInDTO incomingDTO);
    }
}