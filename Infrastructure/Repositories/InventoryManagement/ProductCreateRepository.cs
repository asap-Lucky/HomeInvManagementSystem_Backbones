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
    public class ProductCreateRepository : IProductCreateRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;

        public ProductCreateRepository(HomeinvsystemContext context)
        {
            _context = context;
        }

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

                var createOutDto = await SetCreateproductDto(savedProduct);

                return createOutDto;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // Sets the return product DTO after creation to the outbound DTO.
        private async Task<CreateProductOutDTO> SetCreateproductDto(Product savedProduct)
        {
            try
            {
                var createOutDto = new CreateProductOutDTO
                {
                    ProductId = savedProduct.Id,
                    ProductName = savedProduct.Name,
                    EanCode = savedProduct.Barcode,
                    Brand = savedProduct.Brand,
                    ExpirationDate = savedProduct.ExpiresAt,
                    CreatedAt = savedProduct.CreatedAt,
                    UpdatedAt = savedProduct.UpdatedAt,
                    ImageBLOB = Encoding.UTF8.GetString(savedProduct?.Image?.Data) ?? null
                };

                // Set the category name from category id due to otherwise having to update in database and in API if new categories are added.
                // This does call the database again, but only once per creation, so should be fine for now.
                if (savedProduct.CategoryId != null)
                {
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(categories => categories.Id == savedProduct.CategoryId);

                    if (category is null)
                    {
                        throw new Exception($"Invalid category id '{savedProduct.CategoryId}' supplied.");
                    }

                    createOutDto.Category = category?.Name;
                }

                if (savedProduct.ProductLocations != null && savedProduct.ProductLocations.Count != 0)
                {
                    foreach (var location in savedProduct.ProductLocations)
                    {
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
    }
}
