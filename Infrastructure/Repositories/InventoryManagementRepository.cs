using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class InventoryManagementRepository : IInventoryManagementRepository
    {
        // Injections
        private readonly ILogger<InventoryManagementRepository> _logger;
        private readonly HomeinvsystemContext _context;

        public InventoryManagementRepository(ILogger<InventoryManagementRepository> logger, HomeinvsystemContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<CreateProductOutDTO> AddProductToInventoryAsync(CreateProductInDTO createInDTO)
        {
            try
            {
                var product = _context.Products.Add(new Models.HomeInv.Product
                {
                    Name = createInDTO.ProductName,
                    CategoryId = createInDTO.Category,
                    Barcode = createInDTO.EANCode,
                    Brand = createInDTO.Brand,
                    ExpiresAt = createInDTO.ExpirationDate,
                    Image = createInDTO.ImageBLOB != null ? new Models.HomeInv.Image
                    {
                        Data = Encoding.UTF8.GetBytes(createInDTO.ImageBLOB)                   
                    } : null
                });

                var entry = _context.Products.Add(product.Entity);
                await _context.SaveChangesAsync();

                var savedProduct = await _context.Products
                                .Include(p => p.Image)
                                .Include(p => p.ProductLocations)
                                .Include(p => p.Countries)
                                .Include(p => p.Suppliers)
                                .Include(p => p.Tags)
                                .FirstAsync(p => p.Id == entry.Entity.Id);

                var createOutDto = new CreateProductOutDTO
                {
                    ProductId = savedProduct.Id,
                    ProductName = savedProduct.Name,
                    Category = savedProduct.CategoryId,
                    EanCode = savedProduct.Barcode,
                    Brand = savedProduct.Brand,
                    ExpirationDate = savedProduct.ExpiresAt,
                    CreatedAt = savedProduct.CreatedAt,
                    UpdatedAt = savedProduct.UpdatedAt,

                    Locations = savedProduct.ProductLocations
                                .Select(x => x.LocationId)
                                .ToList(),

                    CountriesOfOrigin = savedProduct.Countries
                                .Select(x => x.Name)
                                .ToList(),

                    Suppliers = savedProduct.Suppliers
                                .Select(x => x.Name)
                                .ToList(),

                    ImageBLOB = Convert.ToBase64String(savedProduct?.Image?.Data),
                    Tags = savedProduct.Tags.Select(x => x.Name).ToList()
                };

                return createOutDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<CreateProductInDTO>> AddProductsBulkToInventoryAsync(List<CreateProductInDTO> createBulkInDTO)
        {
            try
            {
                // TODO: Imp base response wrapper later to set some values that are needed later.
                // Implementation for adding product to inventory goes here.
                throw new NotImplementedException("AddProductToInventoryAsync is not yet implemented.");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
