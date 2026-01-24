using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Commands
{
    public interface IInventoryCommand
    {
        Task<CreateProductOutDTO> AddProductToInventoryAsync(CreateProductInDTO createProdInDTO);

        Task AddMockProductsAsync(int mockAmmount);

        Task<UpdateProductDetailsOutDTO> UpdateProductDetailsAsync(UpdateProductDetailsInDTO updateProdInDTO);

        Task<UpdateLocationStockOutDTO> UpdateLocationStockAsync(UpdateLocationStockInDTO updateLocationStockInDTO);
    }
}
