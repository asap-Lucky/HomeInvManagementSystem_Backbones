using Application.DTOs;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.InventoryManagement
{
    public class ProductReadRepository : IProductReadRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<ProductReadRepository> _logger;

        public ProductReadRepository(HomeinvsystemContext context, ILogger<ProductReadRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<ProductAggregateDTO>> GetProductsFromInventoryAsync(Domain.Enums.ProductLocation location, bool getAllLocations = false)
        {
            try
            {
                List<Models.HomeInv.Product> products = new();

                if (getAllLocations)
                {
                    products = await _context.Products
                        .Include(p => p.ProductLocations)
                        .Include(p => p.Countries)
                        .Include(p => p.Tags)
                        .Include(p => p.Suppliers)
                        .ToListAsync();
                }
                else
                {
                    products = await _context.Products
                        .Include(p => p.ProductLocations)
                        .Include(p => p.Countries)
                        .Include(p => p.Tags)
                        .Include(p => p.Suppliers)
                        .Where(p => p.ProductLocations.Any(pl => pl.LocationId == (int)location))
                        .ToListAsync();
                }

                var productListDTO = new List<ProductAggregateDTO>();

                // Get list of categories from db.
                var categories = await _context.Categories.ToListAsync();

                foreach (var product in products)
                {
                    var productDTO = new ProductAggregateDTO
                    {
                        ProductId = product.Id,
                        EanCode = product.Barcode,
                        ProductName = product.Name,
                        Category = categories?.FirstOrDefault(ctgr => ctgr.Id == product?.CategoryId)?.Name,
                        Brand = product.Brand,
                        CreatedAt = product.CreatedAt,
                        UpdatedAt = product.UpdatedAt
                    };

                    productDTO.Locations = product.ProductLocations.Select(pl => new ProductLocationDTO
                    {
                        LocationId = pl.LocationId,
                        LocationName = _context.Locations
                                              .Where(l => l.Id == pl.LocationId)
                                              .Select(l => l.Name)
                                              .FirstOrDefault() ?? string.Empty,
                        Quantity = pl.Amount
                    }).ToList();

                    productDTO.CountriesOfOrigin = product.Countries.Select(cuntryOri => new ProductCountryDTO
                    {
                        CountryId = cuntryOri.Id,
                        CountryName = cuntryOri.Name
                    }).ToList();

                    productDTO.Tags = product.Tags.Select(tag => new ProductTagDTO
                    {
                        TagId = tag.Id,
                        TagName = tag.Name
                    }).ToList();

                    productDTO.Suppliers = product.Suppliers.Select(supplier => new ProductSupplierDTO
                    {
                        SupplierId = supplier.Id,
                        SupplierName = supplier.Name
                    }).ToList();

                    productListDTO.Add(productDTO);
                }

                return productListDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving products from inventory.");
                throw;
            }
        }

        public async Task<ProductAggregateDTO> GetProductFromInventoryAsync(Domain.Enums.ProductLocation location, string? eanCode = null)
        {
            try
            {
                // Get list of products in the specified location from inventory.
                var product = await _context.Products
                    .Include(p => p.ProductLocations)
                    .Include(p => p.Countries)
                    .Include(p => p.Tags)
                    .Include(p => p.Suppliers)
                    .Where(p => p.ProductLocations.Any(pl => pl.LocationId == (int)location) && p.Barcode == eanCode)
                    .SingleOrDefaultAsync();

                // Get list of categories from db.
                var categories = await _context.Categories.ToListAsync();

                var productDTO = new ProductAggregateDTO
                {
                    ProductId = product.Id,
                    EanCode = product.Barcode,
                    ProductName = product.Name,
                    ImageBLOB = product.Image != null ? Encoding.UTF8.GetString(product.Image.Data) : null,
                    Category = categories?.FirstOrDefault(ctgr => ctgr.Id == product?.CategoryId)?.Name,
                    Brand = product.Brand,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt
                };

                productDTO.Locations = product.ProductLocations.Select(pl => new ProductLocationDTO
                {
                    LocationId = pl.LocationId,
                    LocationName = _context.Locations
                                          .Where(l => l.Id == pl.LocationId)
                                          .Select(l => l.Name)
                                          .FirstOrDefault() ?? string.Empty,
                    Quantity = pl.Amount
                }).ToList();

                productDTO.CountriesOfOrigin = product.Countries.Select(cuntryOri => new ProductCountryDTO
                {
                    CountryId = cuntryOri.Id,
                    CountryName = cuntryOri.Name
                }).ToList();

                productDTO.Tags = product.Tags.Select(tag => new ProductTagDTO
                {
                    TagId = tag.Id,
                    TagName = tag.Name
                }).ToList();

                productDTO.Suppliers = product.Suppliers.Select(supplier => new ProductSupplierDTO
                {
                    SupplierId = supplier.Id,
                    SupplierName = supplier.Name
                }).ToList();

                return productDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving a product from inventory.");
                throw;
            }
        }
    }
}
