using Application.DTOs;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;
using Infrastructure.Data;
using Infrastructure.Models.HomeInv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

        public async Task<UpdateProductDetailsOutDTO?> UpdateProductDetailsDBAsync(string productId, UpdateProductDetailsInDTO inDTO)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Suppliers)
                    .Include(p => p.Category)
                    .Include(p => p.Image)
                    .Include(p => p.Tags)
                    .Include(p => p.Countries)
                    .FirstOrDefaultAsync(p => p.Id.ToString() == productId);

                if (product == null)
                    return null;

                product.Name = inDTO.ProductName;
                product.Barcode = inDTO.Barcode;
                product.Brand = inDTO.Brand;
                product.UpdatedAt = DateTime.Now;
                product.ImageId = inDTO.ImageId;
                product.CategoryId = inDTO.CategoryId;

                product = await UpdateComplexRelationships(product, inDTO);

                await _context.SaveChangesAsync();

                var outDTO = MapOutDTO(product);

                return outDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating product details in the database.");
                throw;
            }
        }

        private async Task<Product> UpdateComplexRelationships(Product product, UpdateProductDetailsInDTO inDTO)
        {
            try
            {
                // Update suppliers //
                var existingSupplierIds = product.Suppliers.Select(s => s.Id).ToHashSet();
                var incomingSupplierIds = inDTO?.Suppliers?.ToHashSet();

                // Remove missing suppliers
                product.Tags.ToList().RemoveAll(t => !incomingSupplierIds.Contains(t.Id));

                // Add new suppliers
                var suppliersToAdd = await _context.Suppliers.Where(t => incomingSupplierIds
                                                        .Except(existingSupplierIds).Contains(t.Id))
                                                        .ToListAsync();
                
                foreach (var supplier in suppliersToAdd)
                    product.Suppliers.Add(supplier);

                // Update tags //
                var existingTagIds = product.Tags.Select(t => t.Id).ToHashSet();
                var incomingTagIds = inDTO?.Tags?.ToHashSet();

                // Remove missing tags
                product.Tags.ToList().RemoveAll(t => !incomingTagIds.Contains(t.Id));
                // Add new tags
                var tagsToAdd = await _context.Tags.Where(t => incomingTagIds
                                                        .Except(existingTagIds).Contains(t.Id))
                                                        .ToListAsync();

                foreach (var tag in tagsToAdd)
                    product.Tags.Add(tag);

                // Update countries //
                var existingCountryIds = product.Countries.Select(c => c.Id).ToHashSet();
                var incomingCountryIds = inDTO?.OriginCountries?.ToHashSet();

                // Remove missing countries
                product.Countries.ToList().RemoveAll(c => !incomingCountryIds.Contains(c.Id));
                
                // Add new countries
                var countriesToAdd = await _context.Countries.Where(c => incomingCountryIds
                                                        .Except(existingCountryIds).Contains(c.Id))
                                                        .ToListAsync();

                foreach (var country in countriesToAdd)
                    product.Countries.Add(country);

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating complex product relationships in the database.");
                throw;
            }
        }

        private UpdateProductDetailsOutDTO MapOutDTO(Product product)
        {
            try
            {
                UpdateProductDetailsOutDTO outDTO = new()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Barcode = product.Barcode,
                    Brand = product.Brand,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    ImageId = product.ImageId,
                    Category = product.Category != null ? new ProductCategoryDTO
                    {
                        CategoryId = product.Category.Id,
                        CategoryName = product.Category.Name
                    } : null,
                    OriginCountries = product.Countries != null ? product.Countries.Select(c => new ProductCountryDTO
                    {
                        CountryId = c.Id,
                        CountryName = c.Name
                    }).ToList() : null,
                    Suppliers = product.Suppliers != null ? product.Suppliers.Select(s => new ProductSupplierDTO
                    {
                        SupplierId = s.Id,
                        SupplierName = s.Name
                    }).ToList() : null,
                    Tags = product.Tags != null ? product.Tags.Select(t => new ProductTagDTO
                    {
                        TagId = t.Id,
                        TagName = t.Name
                    }).ToList() : null
                };

                return outDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while mapping the product database model to the outgoing DTO.");
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
                        throw new Exception("Invalid LocationId");

                    if (!productExists)
                        throw new Exception("Invalid ProductId");

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

        public async Task<BatchUpdateLocationStockOutDTO> BatchUpdateLocationStockDBAsync(BatchUpdateLocationStockInDTO request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == request.LocationId) ??
                               throw new Exception($"Location not found with the location id: {request.LocationId}");

                // Validation for if all products being updated exist.
                await ValidateProductsExistingAsync(request.StockDeltas);

                var productIds = request.StockDeltas.Select(x => x.ProductId)
                                                        .ToList();

                // Get the all product stocks on the current location
                var locationStocks = await _context.ProductLocations.Where(pl => pl.LocationId == request.LocationId && productIds.Contains(pl.ProductId))
                                                                    .ToListAsync();

                // Returning DTO
                var response = new BatchUpdateLocationStockOutDTO
                {
                    Location = new ProductLocationDTO()
                    {
                        LocationId = location.Id,
                        LocationName = location.Name
                    },   
                    TransactionId = transaction.TransactionId.ToString()
                };

                foreach (var stockChange in request.StockDeltas)
                {
                    var stockRow = locationStocks.FirstOrDefault(x => x.ProductId == stockChange.ProductId);

                    if (stockRow == null && stockChange.Delta < 0)
                        throw new Exception($"Cannot remove stock from a location when no existing stock record is present. Product id: {stockChange.ProductId}");

                    // No existing stock previously -> Add to location
                    if (stockRow == null)
                    {
                        stockRow = new Models.HomeInv.ProductLocation()
                        {
                            LocationId = request.LocationId,
                            ProductId = stockChange.ProductId,
                            Quantity = 0,
                            UpdatedAt = DateTime.Now
                        };

                        _context.ProductLocations.Add(stockRow);
                        locationStocks.Add(stockRow);
                    }

                    var openingStock = stockRow.Quantity;
                    var closingStock = openingStock + stockChange.Delta;

                    // Throw if closing stock is in the negative.
                    if (closingStock < 0)
                        throw new Exception($"Insufficient stock for productId {stockRow.ProductId}. OpeningStock={openingStock}, Delta={stockChange.Delta}");

                    stockRow.Quantity = closingStock;
                    stockRow.UpdatedAt = DateTime.Now;

                    response.StockDeltas.Add(new StockDeltaItemOutDTO()
                    {
                        ProductId = stockChange.ProductId,
                        Delta = stockChange.Delta,
                        OpeningStock = openingStock,
                        ClosingStock = closingStock
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return response;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, $"An error occurred while performing batch update of product quantities on location in the database. Transaction Id {transaction.TransactionId.ToString()}");
                throw;
            }
        }

        private async Task ValidateProductsExistingAsync(List<StockDeltaItemInDTO> incomingStockDTO)
        {
            try
            {
                var requestedProductIds = incomingStockDTO.Select(x => x.ProductId)
                                                          .Distinct()
                                                          .ToList();

                // Fetch products that exist
                var existingProductIds = await _context.Products.Where(x => requestedProductIds.Contains(x.Id))
                                                                .Select(x => x.Id)
                                                                .ToListAsync();

                var missingProductsIds = requestedProductIds.Except(existingProductIds)
                                                            .ToList();

                if (missingProductsIds.Count() > 0)
                    throw new Exception($"Invalid ProductId(s): {string.Join(", ", missingProductsIds)}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to validate products existing when batch updating.");
                throw;
            }
        }
    }
}