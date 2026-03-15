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
        private readonly IProductDeleteRepository _deleteProductRepo;

        public InventoryCommand(IProductCreateRepository createProductRepo, IProductUpdateRepository updateProductRepo, IProductDeleteRepository productDeleteRepo)
        {
            _createProductRepo = createProductRepo;
            _updateProductRepo = updateProductRepo;
            _deleteProductRepo = productDeleteRepo;
        }

        public async Task<CreateProductOutDTO> CreateProduct(CreateProductInDTO createProdInDTO)
        {
            try
            {
                var createProduct = await _createProductRepo.CreateInventoryProductAsync(createProdInDTO);
                return createProduct;
            }
            catch
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
            catch
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
            catch
            {
                throw;
            }
        }

        public async Task<BatchUpdateLocationStockOutDTO> BatchUpdateLocationStockAsync(BatchUpdateLocationStockInDTO dto)
        {
            try
            {
                // Combine all copies of products with same product id.
                dto.StockDeltas = dto.StockDeltas.GroupBy(sd => sd.ProductId)
                                    .Select(g => new StockDeltaItemInDTO
                                    {
                                        ProductId = g.Key,
                                        Delta = g.Sum(sd => sd.Delta)
                                    })
                                   .ToList();

                var updateProducts = await _updateProductRepo.BatchUpdateLocationStockDBAsync(dto);
                return updateProducts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<DeleteProductOutDTO> DeleteProductFromInventoryAsync(DeleteProductInDTO dto)
        {
            try
            {
                var deletedProduct = await _deleteProductRepo.DeleteProductOnInventoryDBAsync(dto);
                return deletedProduct;
            }
            catch
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
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
