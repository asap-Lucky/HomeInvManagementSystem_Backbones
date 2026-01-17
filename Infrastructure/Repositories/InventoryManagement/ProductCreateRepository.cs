using Application.DTOs;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;
using Infrastructure.Data;
using Infrastructure.Models.HomeInv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Infrastructure.Repositories.InventoryManagement
{
    public class ProductCreateRepository : IProductCreateRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<ProductCreateRepository> _logger;

        public ProductCreateRepository(HomeinvsystemContext context, ILogger<ProductCreateRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CreateProductOutDTO> AddProductToInventoryDBAsync(CreateProductInDTO createInDTO)
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
                    ImageId = createInDTO.ImageId,
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

                if (createInDTO.OriginCountries != null)
                {
                    var countryListDB = await _context.Countries.ToListAsync();

                    foreach (var countryId in createInDTO.OriginCountries)
                    {
                        var countryDB = countryListDB.FirstOrDefault(c => c.Id == countryId) ?? 
                            throw new Exception($"Country with id {countryId} not found in database!");

                        product.Countries.Add(new Models.HomeInv.Country
                        {
                            Id = countryDB.Id,
                            Name = countryDB.Name
                        });
                    }
                }

                if (createInDTO.Suppliers != null)
                {
                    var supplierListDB = await _context.Suppliers.ToListAsync();

                    foreach (var supplierId in createInDTO.Suppliers)
                    {
                        var supplierDB = supplierListDB.FirstOrDefault(s => s.Id == supplierId) ??
                            throw new Exception($"Supplier with id {supplierId} not found in database");
                        // If not found - What then to do and how to communicate to the user the upload was successful but some countries were invalid?

                        product.Suppliers.Add(new Models.HomeInv.Supplier
                        {
                            Id = supplierDB.Id,
                            Name = supplierDB.Name
                        });
                    }
                }

                if (createInDTO.Tags != null)
                {
                    var tagListDB = await _context.Tags.ToListAsync();

                    foreach (var tagId in createInDTO.Tags)
                    {
                        var tagDB = tagListDB.FirstOrDefault(t => t.Id == tagId) ??
                            throw new Exception($"Tag with id {tagId} not found in database");

                        product.Tags.Add(new Models.HomeInv.Tag
                        {
                            Id = tagDB.Id,
                            Name = tagDB.Name
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
                _logger.LogError(ex, "Error occurred while adding product to inventory.");
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
                    ImageId = savedProduct.ImageId
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
                            Quantity = location.Amount
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while setting CreateProductOutDTO.");
                throw;
            }
        }

        public async Task AddMockProductsToDBAsync(int mockAmmount)
        {
            try
            {
                Random rnd = new();

                var productNames = new List<string> { "Milk", "Bread", "Eggs", "Butter", "Cheese", "Yogurt", "Apples", "Bananas", "Chicken", "Beef" };
                var brands = new List<string> { "BrandA", "BrandB", "BrandC", "BrandD" };
                var locations = await _context.Locations.ToListAsync();
                var tags = await _context.Tags.ToListAsync();
                var suppliers = await _context.Suppliers.ToListAsync();
                var categories = await _context.Categories.ToListAsync();
                var countries = await _context.Countries.ToListAsync();

                for (int i = 0; i < mockAmmount; i++)
                {
                    var productIn = new CreateProductInDTO
                    {
                        ProductName = productNames[rnd.Next(productNames.Count)],
                        Category = categories[rnd.Next(categories.Count)].Id,
                        EANCode = rnd.Next(111111111, 999999999).ToString(),
                        Brand = brands[rnd.Next(brands.Count)],
                        ExpirationDate = DateTime.UtcNow.AddDays(rnd.Next(1, 365)),
                        ImageId = rnd.Next(1, 5),
                        Locations = new List<int> { locations[rnd.Next(locations.Count)].Id },
                        Tags = new List<int> { tags[rnd.Next(tags.Count)].Id },
                        Suppliers = new List<int> { suppliers[rnd.Next(suppliers.Count)].Id },
                        OriginCountries = new List<int> { countries[rnd.Next(countries.Count)].Id }
                    };

                    await AddProductToInventoryDBAsync(productIn);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding mock products to inventory.");
                throw;
            }
        }
    }
}
