using Application.DTOs;
using Application.DTOs.Outbound;
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

        public async Task<List<GetProductOutDTO>> GetProductsAsync()
        {
            try
            {
                var products = await _context.Products.Include(p => p.ProductLocations)
                                                      .Include(p => p.Category)
                                                      .Include(p => p.Countries)
                                                      .Include(p => p.Tags)
                                                      .Include(p => p.Suppliers)
                                                      .ToListAsync();

                List<GetProductOutDTO> outDTOs = new();

                foreach (var product in products)
                {
                    var outDTO = MapModelToDTO(product);
                    outDTOs.Add(outDTO);
                }

                return outDTOs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to retrieve a list of products in inventory.");
                throw;
            }
        }

        public async Task<List<GetProductOutDTO>> GetProductsByLocationIdAsync(string id)
        {
            try
            {
                var products = await _context.Products.Include(p => p.ProductLocations)
                                                      .Include(p => p.Category)
                                                      .Include(p => p.Countries)
                                                      .Include(p => p.Tags)
                                                      .Include(p => p.Suppliers)
                                                      .Where(p => p.ProductLocations.Any(loc => loc.Location.Id.ToString() == id))
                                                      .ToListAsync();

                List<GetProductOutDTO> outDTOs = new();

                foreach (var product in products)
                {
                    var outDTO = MapModelToDTO(product);
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

        public async Task<GetProductOutDTO?> GetProductByIdAsync(string id)
        {
            try
            {
                var product = await _context.Products.Include(p => p.ProductLocations)
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

                    Category = new()
                    {
                        CategoryId = product.Category.Id,
                        CategoryName = product.Category.Name
                    },

                    CountriesOfOrigin = product.Countries?.Select(c => new ProductCountryDTO
                    {
                        CountryId = c.Id,
                        CountryName = c.Name
                    }).ToList(),

                    Suppliers = product.Suppliers?.Select(s => new ProductSupplierDTO()
                    {
                        SupplierId = s.Id,
                        SupplierName = s.Name
                    }).ToList(),

                    Tags = product.Tags?.Select(t => new ProductTagDTO()
                    {
                        TagId = t.Id,
                        TagName = t.Name
                    }).ToList(),

                    Locations = product.ProductLocations?.Select(pl => new ProductLocationDTO
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

        public async Task<ProductLocationDTO> GetLocationByIdAsync(int id)
        {
            try
            {
                var location = await _context.Locations.FirstOrDefaultAsync(loc => loc.Id == id);

                if (location == null)
                    return new ProductLocationDTO();
           
                return new ProductLocationDTO()
                {
                    LocationId = location.Id,
                    LocationName = location.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to fetch the location by location ID.");
                throw;
            }
        }
    }
}
