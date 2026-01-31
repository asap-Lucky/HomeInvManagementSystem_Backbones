using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Repositories;
using Application.Interfaces.Repositories.InventoryManagement;
using Microsoft.Extensions.Logging;

namespace Application.Commands
{
    public class InventoryCommand : IInventoryCommand
    {
        // Injections
        private readonly IProductCreateRepository _createProductRepo;
        private readonly IProductUpdateRepository _updateProductRepo;

        public InventoryCommand(IProductCreateRepository createProductRepo, IProductUpdateRepository updateProductRepo)
        {
            _createProductRepo = createProductRepo;
            _updateProductRepo = updateProductRepo;
        }

        public async Task<CreateProductOutDTO> AddProductToInventoryAsync(CreateProductInDTO createProdInDTO)
        {
            try
            {
                var createProduct = await _createProductRepo.AddProductToInventoryDBAsync(createProdInDTO);
                return createProduct;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UpdateProductDetailsOutDTO> UpdateProductDetailsAsync(UpdateProductDetailsInDTO updateProdInDTO)
        {
            try
            {
                var updateProduct = await _updateProductRepo.UpdateProductDetailsDBAsync(updateProdInDTO);
                return updateProduct;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UpdateLocationStockOutDTO> UpdateLocationStockAsync(UpdateLocationStockInDTO updateLocationStockInDTO)
        {
            try
            {
                var updateProduct = await _updateProductRepo.UpdateLocationStockDBAsync(updateLocationStockInDTO);
                return updateProduct;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BatchUpdateLocationStockOutDTO> BatchUpdateLocationStockAsync(BatchUpdateLocationStockInDTO batchUpdateLocationStockInDTO)
        {
            try
            {
                var updateProducts = await _updateProductRepo.BatchUpdateLocationStockDBAsync(batchUpdateLocationStockInDTO);
                return updateProducts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Mocked Methods
        public async Task AddMockProductsAsync(int mockAmmount)
        {
            try
            {
                await _createProductRepo.AddMockProductsToDBAsync(mockAmmount);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
