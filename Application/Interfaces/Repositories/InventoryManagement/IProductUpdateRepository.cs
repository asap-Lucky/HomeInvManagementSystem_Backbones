using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.InventoryManagement
{
    public interface IProductUpdateRepository
    {
        Task<UpdateProductDetailsOutDTO> UpdateProductDetailsDBAsync(UpdateProductDetailsInDTO incomingDTO);

        Task<UpdateLocationStockOutDTO> UpdateLocationStockDBAsync(UpdateLocationStockInDTO incomingDTO);
    }
}
