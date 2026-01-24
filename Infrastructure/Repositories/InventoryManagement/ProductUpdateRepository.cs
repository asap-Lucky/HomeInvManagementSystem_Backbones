using Application.DTOs;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.InventoryManagement
{
    public class ProductUpdateRepository : IProductUpdateRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;

        private readonly ILogger<ProductUpdateRepository> _logger;

        public ProductUpdateRepository(HomeinvsystemContext context, ILogger<ProductUpdateRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateProductDetailsOutDTO> UpdateProductDetailsDBAsync(int productId, UpdateProductDetailsInDTO incomingDTO)
        {
            try
            {
                if (incomingDTO == null)
                    throw new Exception("Could not update product due to missing incoming data.");

                var productDB = await _context.Products
                    .Include(p => p.Suppliers)
                    .Include(p => p.Category)
                    .Include(p => p.Image)
                    .Include(p => p.Tags)
                    .Include(p => p.Countries)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (productDB == null)
                    throw new Exception("Could not update product due to id not existing in system.");

                // Update fields in DB entity.
                productDB.Name = incomingDTO.ProductName;
                productDB.Barcode = incomingDTO.EanCode;
                productDB.Brand = incomingDTO.Brand;
                productDB.UpdatedAt = DateTime.Now;
                productDB.Image = _context.Images?.FirstOrDefault(i => i.Id == incomingDTO.ImageId);
                productDB.ImageId = incomingDTO.ImageId;
                productDB.Category = _context.Categories.First(c => c.Id == incomingDTO.CategoryId)!;

                // Clear existing complex relations.
                productDB.Suppliers.Clear();
                productDB.Tags.Clear();
                productDB.Countries.Clear();

                await _context.SaveChangesAsync();

                // Set complex relations.
                productDB.Suppliers = incomingDTO.Suppliers != null ? incomingDTO?.Suppliers?.Select(s => _context.Suppliers.FirstOrDefault(sup => sup.Id == s)).ToList() : new List<Models.HomeInv.Supplier>();
                productDB.Tags = incomingDTO.Tags != null ? incomingDTO.Tags?.Select(t => _context.Tags.FirstOrDefault(tag => tag.Id == t)).ToList() : new List<Models.HomeInv.Tag>();
                productDB.Countries = incomingDTO.OriginCountries != null ? incomingDTO.OriginCountries.Select(c => _context.Countries.FirstOrDefault(country => country.Id == c)).ToList() : new List<Models.HomeInv.Country>();

                await _context.SaveChangesAsync();

                var outgoingDTO = MapDBModelToOutgoingDTO(productDB);

                return outgoingDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product details in the database.");
                throw;
            }
        }

        private UpdateProductDetailsOutDTO MapDBModelToOutgoingDTO(Models.HomeInv.Product productDB)
        {
            try
            {
                UpdateProductDetailsOutDTO outgoingDTO = new UpdateProductDetailsOutDTO();

                outgoingDTO.ProductId = productDB.Id;
                outgoingDTO.ProductName = productDB.Name;
                outgoingDTO.EanCode = productDB.Barcode;
                outgoingDTO.Brand = productDB.Brand;
                outgoingDTO.UpdatedAt = productDB.UpdatedAt;
                outgoingDTO.ImageId = productDB.ImageId;

                outgoingDTO.Category = new ProductCategoryDTO
                {
                    CategoryId = productDB.Category.Id,
                    CategoryName = productDB.Category.Name
                };

                outgoingDTO.OriginCountries = productDB.Countries != null ? productDB.Countries.Select(c => new ProductCountryDTO
                {
                    CountryId = c.Id,
                    CountryName = c.Name
                }).ToList() : new List<ProductCountryDTO>();

                outgoingDTO.Suppliers = productDB.Suppliers != null ? productDB.Suppliers.Select(s => new ProductSupplierDTO
                {
                    SupplierId = s.Id,
                    SupplierName = s.Name
                }).ToList() : new List<ProductSupplierDTO>();

                outgoingDTO.Tags = productDB.Tags != null ? productDB.Tags.Select(t => new ProductTagDTO
                {
                    TagId = t.Id,
                    TagName = t.Name
                }).ToList() : new List<ProductTagDTO>();

                return outgoingDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while mapping the product database model to the outgoing DTO.");
                throw;
            }
        }


    }
}