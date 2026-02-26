using Application.DTOs;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;
using Infrastructure.Data;
using Infrastructure.Models.HomeInv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.InventoryManagement
{
    public class ProductDeleteRepository : IProductDeleteRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<ProductDeleteRepository> _logger;

        public ProductDeleteRepository(HomeinvsystemContext context, ILogger<ProductDeleteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // NOTE: This does not delete the product as so but rather sets it to status "Deleted" so history can be saved of products bought in the future.
        public async Task<DeleteProductOutDTO> DeleteProductOnInventoryDBAsync(DeleteProductInDTO incomingDTO)
        {
            try
            {
                var product = await _context.Products.Include(p => p.ProductLocations)
                                                     .Include(p => p.Countries)
                                                     .Include(p => p.Category)
                                                     .Include(p => p.Tags)
                                                     .Include(p => p.Suppliers)
                                                     .FirstOrDefaultAsync(x => x.Id == incomingDTO.ProductId);

                if (product == null)
                    throw new Exception($"No product with id {incomingDTO.ProductId}");

                var productLocations = _context.ProductLocations.Where(x => x.ProductId == incomingDTO.ProductId && x.Quantity > 0)
                                                                .Include(p => p.Location)
                                                                .ToList();

                productLocations.ForEach(x =>
                {
                    x.Quantity = 0;
                });

                // Set product to isDeleted
                product.IsDeleted = true;

                _context.SaveChanges();

                DeleteProductOutDTO outDTO = new()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    EanCode = product.Barcode,
                    Brand = product.Brand,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = DateTime.Now,
                    Category = new ProductCategoryDTO()
                    {
                        CategoryId = product.Category.Id,
                        CategoryName = product.Category.Name
                    },

                    Locations = product.ProductLocations?.Select(pl => new ProductLocationDTO()
                    {
                        LocationId = pl.LocationId,
                        LocationName = pl.Location.Name
                    })
                    .ToList(),

                    OriginCountries = product.Countries?.Select(oc => new ProductCountryDTO()
                    {
                        CountryId = oc.Id,
                        CountryName = oc.Name
                    })
                    .ToList(),

                    Tags = product.Tags?.Select(t => new ProductTagDTO()
                    {
                        TagId = t.Id,
                        TagName = t.Name
                    })
                    .ToList(),

                    Suppliers = product.Suppliers?.Select(s => new ProductSupplierDTO()
                    {
                        SupplierId = s.Id,
                        SupplierName = s.Name
                    })
                    .ToList()
                };

                return outDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error happened while trying to delete the product with id {incomingDTO.ProductId}");
                throw;
            }
        }
    }
}
