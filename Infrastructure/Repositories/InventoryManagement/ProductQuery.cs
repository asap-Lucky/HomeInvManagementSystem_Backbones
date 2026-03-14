using Application.DTOs;
using Application.DTOs.Outbound;
using Application.Interfaces.Queries;
using Application.Interfaces.Repositories.InventoryManagement;
using Domain.Common.Rules;
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
    public class ProductQuery : IProductQuery
    {
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<ProductQuery> _logger;
        public ProductQuery(HomeinvsystemContext context, ILogger<ProductQuery> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetProductOutDTO?> GetProductByIdAsync(string id)
        {
            try
            {
                var product = await _context.Products.Include(p => p.ProductLocations)
                                                     .ThenInclude(pl => pl.Location)
                                                     .Include(p => p.Countries)
                                                     .Include(p => p.Category)
                                                     .Include(p => p.Tags)
                                                     .Include(p => p.Suppliers)
                                                     .FirstOrDefaultAsync(p => p.Id.ToString() == id);

                if (product == null)
                    return null;

                var outDTO = MapModelToDTO(product);

                return outDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving a product from inventory.");
                throw;
            }
        }

        public async Task<GetProductOutDTO?> GetProductByBarcodeAsync(string barcode)
        {
            try
            {
                var product = await _context.Products.Include(p => p.ProductLocations)
                                                     .ThenInclude(pl => pl.Location)
                                                     .Include(p => p.Countries)
                                                     .Include(p => p.Category)
                                                     .Include(p => p.Tags)
                                                     .Include(p => p.Suppliers)
                                                     .FirstOrDefaultAsync(p => p.Barcode == barcode);

                if (product == null)
                    return null;

                var outDTO = MapModelToDTO(product);

                return outDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving a product from inventory.");
                throw;
            }
        }

        public async Task<List<GetProductLocationOutDTO>?> GetProductsByLocationIdAsync(string id)
        {
            try
            {
                var locationExists = await _context.Locations.AnyAsync(l => l.Id.ToString() == id);

                if (!locationExists)
                    return null;

                List<Product>? products = new();

                // NOTE: This is a special case to get all products regardless of location, as the "All Locations" option has an ID of 20 in the database.
                if (id == "20")
                {
                    products = await _context.Products.Include(p => p.ProductLocations)
                                                      .ThenInclude(pl => pl.Location)
                                                      .ToListAsync();
                }
                else
                {
                    products = await _context.Products.Include(p => p.ProductLocations)
                                                      .ThenInclude(pl => pl.Location)
                                                      .Where(p => p.ProductLocations.Any(loc => loc.Location.Id.ToString() == id))
                                                      .ToListAsync();
                }

                List<GetProductLocationOutDTO> outDTOs = new();

                foreach (var product in products)
                {
                    var outDTO = new GetProductLocationOutDTO
                    {
                        ProductId = product.Id.ToString(),
                        ProductName = product.Name,
                        Stock = product.ProductLocations.FirstOrDefault(pl => pl.LocationId.ToString() == id)?.Quantity ?? 0
                    };

                    if (outDTO != null)
                        outDTOs.Add(outDTO);
                }

                return outDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving products from inventory on location ID: {id}");
                throw;
            }
        }

        private GetProductOutDTO? MapModelToDTO(Product product)
        {
            try
            {
                GetProductOutDTO outDTO = new()
                {
                    ProductId = product.Id.ToString(),
                    ProductName = product.Name,
                    Barcode = product.Barcode,
                    Brand = product.Brand,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    ImageId = product.ImageId,
                    IsDeleted = product.IsDeleted,

                    Category = product.Category == null ? null : new ProductCategoryDTO
                    {
                        CategoryId = product.Category.Id,
                        CategoryName = product.Category.Name
                    },

                    CountriesOfOrigin = product.Countries == null ? null : product.Countries.Select(c => new ProductCountryDTO
                    {
                        CountryId = c.Id,
                        CountryName = c.Name
                    }).ToList(),

                    Suppliers = product.Suppliers == null ? null : product.Suppliers.Select(s => new ProductSupplierDTO()
                    {
                        SupplierId = s.Id,
                        SupplierName = s.Name
                    }).ToList(),

                    Tags = product.Tags == null ? null : product.Tags.Select(t => new ProductTagDTO()
                    {
                        TagId = t.Id,
                        TagName = t.Name
                    }).ToList(),

                    Locations = product.ProductLocations == null ? null : product.ProductLocations.Select(pl => new ProductLocationDTO
                    {
                        LocationId = pl.LocationId,
                        LocationName = pl.Location.Name,
                        Stock = pl.Quantity
                    }).ToList()
                };

                return outDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to map product model to outgoing DTO.");
                return null;
            }
        }
    }
}
