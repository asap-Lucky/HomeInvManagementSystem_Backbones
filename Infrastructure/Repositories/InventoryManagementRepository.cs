using Application.DTOs;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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

        // Read actions
        public async Task<List<ProductAggregateDTO>> GetProductsFromInventoryAsync(ProductLocation location)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.ProductLocations)
                    .Where(p => p.ProductLocations.Any(pl => pl.LocationId == (int)location))
                    .ToListAsync();

                var productDTOs = new List<ProductAggregateDTO>();

                productDTOs = products.Select(p => new ProductAggregateDTO
                {
                    EanCode = p.Barcode,
                    ProductName = p.Name,
                    Category = p.Category.Name,
                    Brand = p.Brand,
                    ImageBLOB = p.Image != null ? Encoding.UTF8.GetString(p.Image.Data) : null,
                    ExpirationDate = p.ExpiresAt,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Locations = p.ProductLocations.Select(pl => new ProductLocationDTO
                    {
                        LocationId = pl.LocationId,
                        LocationName = _context.Locations
                                              .Where(l => l.Id == pl.LocationId)
                                              .Select(l => l.Name)
                                              .FirstOrDefault() ?? string.Empty,
                        Ammount = pl.Amount
                    }).ToList(),
                    CountriesOfOrigin = p.Countries.Select(cuntryOri => new ProductCountryDTO
                    {
                        CountryId = cuntryOri.Id,
                        CountryName = cuntryOri.Name
                    }).ToList(),
                    Tags = p.Tags.Select(tag => new ProductTagDTO
                    {
                        TagId = tag.Id,
                        TagName = tag.Name
                    }).ToList()
                }).ToList();

                return productDTOs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ProductAggregateDTO> GetProductFromInventoryAsync(ProductLocation location, string? eanCode = null)
        {
            try
            {
                var products = await _context.Products
                    .Include(p => p.ProductLocations)
                    .Where(p => p.ProductLocations
                        .Any(pl => pl.LocationId == (int)location && pl.Product.Barcode == eanCode))
                    .SingleOrDefaultAsync();

                var productDTO = new ProductAggregateDTO
                {
                    EanCode = products.Barcode,
                    ProductName = products.Name,
                    Category = products.Category.Name,
                    Brand = products.Brand,
                    ImageBLOB = products.Image != null ? Encoding.UTF8.GetString(products.Image.Data) : null,
                    ExpirationDate = products.ExpiresAt,
                    CreatedAt = products.CreatedAt,
                    UpdatedAt = products.UpdatedAt,
                    Locations = products.ProductLocations.Select(pl => new ProductLocationDTO
                    {
                        LocationId = pl.LocationId,
                        LocationName = _context.Locations
                                              .Where(l => l.Id == pl.LocationId)
                                              .Select(l => l.Name)
                                              .FirstOrDefault() ?? string.Empty,
                        Ammount = pl.Amount
                    }).ToList(),
                    CountriesOfOrigin = products.Countries.Select(cuntryOri => new ProductCountryDTO
                    {
                        CountryId = cuntryOri.Id,
                        CountryName = cuntryOri.Name
                    }).ToList(),
                    Tags = products.Tags.Select(tag => new ProductTagDTO
                    {
                        TagId = tag.Id,
                        TagName = tag.Name
                    }).ToList()
                };

                return productDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Create actions
        public async Task<CreateProductOutDTO> AddProductToInventoryAsync(CreateProductInDTO createInDTO)
        {
            try
            {
                // Create Primary values in the table.
                var product = new Models.HomeInv.Product
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
                };

                var entry = _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Populate Junction tables 
                if (createInDTO.Locations != null)
                {
                    foreach (var locationId in createInDTO.Locations)
                    {
                        // Updating the count of the ammount in a location.
                        var existingEntry = await _context.ProductLocations
                            .FirstOrDefaultAsync(pl => pl.LocationId == locationId && pl.ProductId == product.Id);

                        if (existingEntry == null)
                        {
                            // Initial creation of none existing entry in DB.
                            _context.ProductLocations.Add(new Models.HomeInv.ProductLocation
                            {
                                ProductId = product.Id,
                                LocationId = locationId,
                                Amount = 1,
                                UpdatedAt = DateTime.UtcNow
                            });
                        }
                        else
                        {
                            existingEntry.Amount += 1;
                            existingEntry.UpdatedAt = DateTime.UtcNow;
                        }
                    }
                }

                if (createInDTO.CountriesOfOrigin != null)
                {
                    foreach (var countryName in createInDTO.CountriesOfOrigin)
                    {
                        product.Countries.Add(new Models.HomeInv.Country
                        {
                            Name = countryName
                        });
                    }
                }

                if (createInDTO.Suppliers != null)
                {
                    foreach (var supplierName in createInDTO.Suppliers)
                    {
                        product.Suppliers.Add(new Models.HomeInv.Supplier
                        {
                            Name = supplierName
                        });
                    }
                }

                if (createInDTO.Tags != null)
                {
                    foreach (var tagName in createInDTO.Tags)
                    {
                        product.Tags.Add(new Models.HomeInv.Tag
                        {
                            Name = tagName
                        });
                    }
                }

                await _context.SaveChangesAsync();

                // Joining back all data to return the created product with all relations.
                var savedProduct = await _context.Products
                                .Include(p => p.Image)
                                .Include(p => p.ProductLocations)
                                .Include(p => p.Countries)
                                .Include(p => p.Suppliers)
                                .Include(p => p.Tags)
                                .FirstAsync(p => p.Id == entry.Entity.Id);

                var createOutDto = SetCreateproductDto(savedProduct);

                return createOutDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        private CreateProductOutDTO SetCreateproductDto(Models.HomeInv.Product savedProduct)
        {
            try
            {
                var createOutDto = new CreateProductOutDTO
                {
                    ProductId = savedProduct.Id,
                    ProductName = savedProduct.Name,
                    Category = savedProduct.Category.Name ?? null,
                    EanCode = savedProduct.Barcode,
                    Brand = savedProduct.Brand,
                    ExpirationDate = savedProduct.ExpiresAt,
                    CreatedAt = savedProduct.CreatedAt,
                    UpdatedAt = savedProduct.UpdatedAt,
                    ImageBLOB = Encoding.UTF8.GetString(savedProduct?.Image?.Data) ?? null
                };

                if (savedProduct.ProductLocations != null && savedProduct.ProductLocations.Count != 0)
                {
                    foreach (var location in savedProduct.ProductLocations)
                    {
                        var test = _context.Locations.Where(x => x.Id != null).ToList();

                        var pl = new ProductLocationDTO
                        {
                            LocationId = location.LocationId,
                            LocationName = _context?.Locations?.Where(l => l.Id == location.LocationId)
                                                             .Select(l => l.Name)
                                                             .FirstOrDefault() ?? throw new Exception("Creation of new product has invalid location set."),
                            Ammount = location.Amount
                        };

                        if (createOutDto.Locations == null)
                            createOutDto.Locations = new List<ProductLocationDTO>();
                        
                        createOutDto.Locations.Add(pl);
                    }
                }

                if (savedProduct.Countries != null && savedProduct.Countries.Count != 0)
                {
                    foreach (var country in savedProduct.Countries)
                    {
                        var pc = new ProductCountryDTO
                        {
                            CountryId = country.Id,
                            CountryName = country.Name
                        };

                        if (createOutDto.CountriesOfOrigin == null)
                            createOutDto.CountriesOfOrigin = new List<ProductCountryDTO>();

                        createOutDto.CountriesOfOrigin.Add(pc);
                    }
                }

                if (savedProduct.Suppliers != null && savedProduct.Suppliers.Count != 0)
                {
                    foreach (var supplier in savedProduct.Suppliers)
                    {
                        var ps = new ProductSupplierDTO
                        {
                            SupplierId = supplier.Id,
                            SupplierName = supplier.Name
                        };

                        if (createOutDto.Suppliers == null)
                            createOutDto.Suppliers = new List<ProductSupplierDTO>();

                        createOutDto.Suppliers.Add(ps);
                    }
                }

                if (savedProduct.Tags != null && savedProduct.Tags.Count != 0)
                {
                    foreach (var tag in savedProduct.Tags)
                    {
                        var pt = new ProductTagDTO
                        {
                            TagId = tag.Id,
                            TagName = tag.Name
                        };

                        if (createOutDto.Tags == null)
                            createOutDto.Tags = new List<ProductTagDTO>();

                        createOutDto.Tags.Add(pt);
                    }
                }

                return createOutDto;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<CreateProductOutDTO>> AddProductsBulkToInventoryAsync(List<CreateProductInDTO> createBulkInDTO)
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

        // Update actions


        // Delete actions
    }
}
