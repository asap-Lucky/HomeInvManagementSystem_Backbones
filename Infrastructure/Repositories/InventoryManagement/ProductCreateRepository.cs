using Application.DTOs;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;
using Infrastructure.Data;
using Infrastructure.Models.HomeInv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task<CreateProductOutDTO> CreateInventoryProductAsync(CreateProductInDTO inDTO)
        {
            try
            {
                _logger.LogInformation($"[CREATE]: Adding new product to inventory.");

                // Instance of product
                Product prod = new()
                {
                    Name = inDTO.ProductName,
                    Barcode = inDTO.Barcode,
                    Brand = inDTO.Brand,
                    CategoryId = inDTO.CategoryId,
                    ImageId = inDTO.ImageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Products.Add(prod);

                prod.Category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == inDTO.CategoryId)
                        ?? throw new KeyNotFoundException($"The category with the ID: {inDTO.CategoryId} was not found");

                prod.ProductLocations = inDTO.Locations
                    .Select(locationId => new ProductLocation
                    {
                        LocationId = locationId,
                        Quantity = 0,
                        UpdatedAt = DateTime.UtcNow
                    }).ToList();

                if (inDTO.Suppliers != null)
                {
                    var suppliers = await _context.Suppliers
                        .Where(s => inDTO.Suppliers.Contains(s.Id))
                        .ToListAsync();

                    if (suppliers.Count() != inDTO.Suppliers.Count())
                    {
                        var foundIds = suppliers.Select(t => t.Id);
                        var missingIds = inDTO.Suppliers.Where(id => !foundIds.Contains(id));

                        _logger.LogWarning($"[CREATE]: Some suppliers were not found for the new product. Missing Tag IDs: {string.Join(", ", foundIds)}");
                    }

                    prod.Suppliers = suppliers;
                }

                if (inDTO.Tags != null)
                {
                    var tags = await _context.Tags
                        .Where(t => inDTO.Tags.Contains(t.Id))
                        .ToListAsync();

                    if (tags.Count() != inDTO.Tags.Count())
                    {
                        var foundIds = tags.Select(t => t.Id);
                        var missingIds = inDTO.Tags.Where(id => !foundIds.Contains(id));

                        _logger.LogWarning($"[CREATE]: Some tags were not found for the new product. Missing Tag IDs: {string.Join(", ", foundIds)}");
                    }

                    prod.Tags = tags;
                }

                if (inDTO.OriginCountries != null)
                {
                    var countries = await _context.Countries
                        .Where(c => inDTO.OriginCountries.Contains(c.Id))
                        .ToListAsync();

                    if (countries.Count() != inDTO.OriginCountries.Count())
                    {
                        var foundIds = countries.Select(t => t.Id);
                        var missingIds = inDTO.OriginCountries.Where(id => !foundIds.Contains(id));

                        _logger.LogWarning($"[CREATE]: Some countries were not found for the new product. Missing Tag IDs: {string.Join(", ", foundIds)}");
                    }

                    prod.Countries = countries;
                }

                await _context.SaveChangesAsync();

                // Joining back all data to return the created product with all relations.
                var savedProduct = await _context.Products
                                .Include(p => p.Image)
                                .Include(p => p.ProductLocations)
                                .Include(p => p.Countries)
                                .Include(p => p.Suppliers)
                                .Include(p => p.Tags)
                                .FirstAsync(p => p.Id == prod.Id);

                CreateProductOutDTO createOutDto = MapOutDTO(savedProduct);

                _logger.LogInformation($"[CREATE]: Successfully added new product to inventory with ProductId: {createOutDto.ProductId}.");

                return createOutDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product {@ProductDto}", inDTO);
                throw;
            }
        }

        // Sets the return product DTO after creation to the outbound DTO.
        private CreateProductOutDTO MapOutDTO(Product savedProduct)
        {
            try
            {
                var createOutDto = new CreateProductOutDTO
                {
                    ProductId = savedProduct.Id,
                    ProductName = savedProduct.Name,
                    EanCode = savedProduct.Barcode,
                    Brand = savedProduct.Brand,
                    CreatedAt = savedProduct.CreatedAt,
                    UpdatedAt = savedProduct.UpdatedAt,
                    ImageId = savedProduct.ImageId,
                    Category = savedProduct.Category != null ? new ProductCategoryDTO
                    {
                        CategoryId = savedProduct.Category.Id,
                        CategoryName = savedProduct.Category.Name
                    } : null,
                    Locations = savedProduct.ProductLocations != null ? savedProduct.ProductLocations.Select(pl => new ProductLocationDTO
                    {
                        LocationId = pl.LocationId,
                        LocationName = _context?.Locations?.Where(l => l.Id == pl.LocationId)
                                                             .Select(l => l.Name)
                                                             .FirstOrDefault() ?? throw new Exception("Creation of new product has invalid location set.")
                    }).ToList() : null,
                    CountriesOfOrigin = savedProduct.Countries != null ? savedProduct.Countries.Select(c => new ProductCountryDTO
                    {
                        CountryId = c.Id,
                        CountryName = c.Name
                    }).ToList() : null,
                    Suppliers = savedProduct.Suppliers != null ? savedProduct.Suppliers.Select(s => new ProductSupplierDTO
                    {
                        SupplierId = s.Id,
                        SupplierName = s.Name
                    }).ToList() : null,
                    Tags = savedProduct.Tags != null ? savedProduct.Tags.Select(t => new ProductTagDTO
                    {
                        TagId = t.Id,
                        TagName = t.Name
                    }).ToList() : null
                };

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

                var rndNumb = rnd.Next(100000000, 999999999).ToString();

                var productNames = new List<string> { "Milk", "Bread", "Eggs", "Butter", "Cheese", "Yogurt", "Apples", "Bananas", "Chicken", "Beef" };
                var brands = new List<string> { "BrandA", "BrandB", "BrandC", "BrandD" };
                var locations = await _context.Locations.AsNoTracking().ToListAsync();
                var tags = await _context.Tags.AsNoTracking().ToListAsync();
                var suppliers = await _context.Suppliers.AsNoTracking().ToListAsync();
                var categories = await _context.Categories.AsNoTracking().ToListAsync();
                var countries = await _context.Countries.AsNoTracking().ToListAsync();

                for (int i = 0; i < mockAmmount; i++)
                {
                    var productIn = new CreateProductInDTO
                    {
                        ProductName = productNames[rnd.Next(productNames.Count)],
                        CategoryId = categories[rnd.Next(categories.Count)].Id,
                        Barcode = rnd.Next(111111111, 999999999).ToString(),
                        Brand = brands[rnd.Next(brands.Count)],
                        ImageId = rnd.Next(1, 5),
                        Locations = new List<int> { locations[rnd.Next(locations.Count)].Id },
                        Tags = new List<int> { tags[rnd.Next(tags.Count)].Id },
                        Suppliers = new List<int> { suppliers[rnd.Next(suppliers.Count)].Id },
                        OriginCountries = new List<int> { countries[rnd.Next(countries.Count)].Id }
                    };

                    productIn.ProductName += rndNumb;

                    await CreateInventoryProductAsync(productIn);
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