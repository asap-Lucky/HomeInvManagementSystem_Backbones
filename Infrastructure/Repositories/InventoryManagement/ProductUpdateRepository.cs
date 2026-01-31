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

        public async Task<UpdateProductDetailsOutDTO> UpdateProductDetailsDBAsync(UpdateProductDetailsInDTO incomingDTO)
        {
            try
            {
                var productDB = await _context.Products
                    .Include(p => p.Suppliers)
                    .Include(p => p.Category)
                    .Include(p => p.Image)
                    .Include(p => p.Tags)
                    .Include(p => p.Countries)
                    .FirstOrDefaultAsync(p => p.Id == incomingDTO.ProductId);

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

                UpdateProductDetailsOutDTO MapDBModelToOutgoingDTO(Models.HomeInv.Product productDB)
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

                return outgoingDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product details in the database.");
                throw;
            }
        }

        public async Task<UpdateLocationStockOutDTO> UpdateLocationStockDBAsync(UpdateLocationStockInDTO incomingDTO)
        {
            try
            {
                var pl = await _context.ProductLocations
                    .Include(pl => pl.Location)
                    .FirstOrDefaultAsync(pl => pl.LocationId == incomingDTO.LocationId && pl.ProductId == incomingDTO.ProductId);

                // Validation of existing LocationProduct entry - Otherwise add new entry if not found.
                if (pl == null)
                {
                    var locationExists = await _context.Locations.AnyAsync(l => l.Id == incomingDTO.LocationId);
                    var productExists = await _context.Products.AnyAsync(p => p.Id == incomingDTO.ProductId);

                    if (!locationExists)
                        throw new Exception("Invalid ProductId");

                    if (!productExists)
                        throw new Exception("Invalid LocationId.");

                    pl = new Models.HomeInv.ProductLocation
                    {
                        LocationId = incomingDTO.LocationId,
                        ProductId = incomingDTO.ProductId,
                        Quantity = 0,
                        UpdatedAt = DateTime.Now
                    };

                    _context.ProductLocations.Add(pl);

                    // Load information about the Location for the outgoing DTO.
                    pl.Location = await _context.Locations.FirstAsync(l => l.Id == incomingDTO.LocationId);
                }

                int newQty = pl.Quantity + incomingDTO.Delta;

                if (newQty < 0)
                    throw new Exception("Resulting quantity cannot be negative.");

                pl.Quantity = newQty;
                pl.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return new UpdateLocationStockOutDTO
                {
                    ProductId = pl.ProductId,
                    Location = new ProductLocationDTO
                    {
                        LocationId = pl.Location.Id,
                        LocationName = pl.Location.Name
                    },
                    Quantity = pl.Quantity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product quantity on location in the database.");
                throw;
            }
        }

        public async Task<BatchUpdateLocationStockOutDTO> BatchUpdateLocationStockDBAsync(BatchUpdateLocationStockInDTO incomingDTO)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var loc = await _context.ProductLocations.FirstOrDefaultAsync(l => l.LocationId == incomingDTO.LocationId);

                if (loc == null)
                    throw new Exception("Could not find location for batch update.");

                var transactionId = Guid.NewGuid().ToString();

                foreach (var product in loc.Product)
                {
                    // Find matching stock delta for product.
                }

                _context.s

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while performing batch update of product quantities on location in the database.");
                throw;
            }
        }
    }
}