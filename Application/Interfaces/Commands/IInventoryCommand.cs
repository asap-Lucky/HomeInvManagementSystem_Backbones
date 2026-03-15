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
        Task<CreateProductOutDTO> CreateProduct(CreateProductInDTO createProdInDTO);

        Task<UpdateProductDetailsOutDTO> UpdateProductDetailsAsync(UpdateProductDetailsInDTO updateProdInDTO);

        Task<UpdateLocationStockOutDTO> UpdateLocationStockAsync(UpdateLocationStockInDTO updateLocationStockInDTO);

        #region Mocks
        Task AddMockProductsAsync(int mockAmmount);
        #endregion

        #region Transactions
        Task<BatchUpdateLocationStockOutDTO> BatchUpdateLocationStockAsync(BatchUpdateLocationStockInDTO batchUpdateLocationStockInDTO);
        Task<DeleteProductOutDTO> DeleteProductFromInventoryAsync(DeleteProductInDTO dto);
        #endregion
    }
}
