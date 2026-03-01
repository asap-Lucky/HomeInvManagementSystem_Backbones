using Application.DTOs;
using Application.DTOs.Outbound;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.InventoryManagement
{
    public interface IProductReadRepository
    {
        // TODO: Move this into its own repository.
        Task<ProductLocationDTO> GetLocationByIdAsync(int id);


        Task<GetProductOutDTO?> GetProductByIdAsync(string id);

        Task<GetProductOutDTO?> GetProductByBarcodeAsync(string barcode);

        Task<List<GetProductOutDTO>> GetProductsByLocationIdAsync(string id);
    }
}
