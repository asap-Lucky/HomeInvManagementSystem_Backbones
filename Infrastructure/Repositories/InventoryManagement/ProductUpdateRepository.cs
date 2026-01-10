using Application.DTOs;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Repositories.InventoryManagement;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.InventoryManagement
{
    public class ProductUpdateRepository : IProductUpdateRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;

        public ProductUpdateRepository(HomeinvsystemContext context)
        {
            _context = context;
        }

        public async Task<UpdateProductDetailsOutDTO> UpdateProductDetailsDBAsync(UpdateProductDetailsInDTO incomingDTO)
        {
            try
            {
                var productDB = _context.Products.FirstOrDefault(p => p.Id == incomingDTO.ProductId);

                if (productDB == null)
                    throw new Exception("Could not update product due to id not existing in system.");

                // Update fields in DB entity.
                productDB.Name = incomingDTO.ProductName;
                productDB.Barcode = incomingDTO.EanCode;
                productDB.Brand = incomingDTO.Brand;
                productDB.ExpiresAt = incomingDTO.ExpirationDate;
                productDB.UpdatedAt = DateTime.Now;
                productDB.Category = _context.Categories.FirstOrDefault(c => c.Id == incomingDTO.Category.CategoryId)!;

                // If not picture exsists matching id create a new one in the DB.
                productDB.Image = incomingDTO.Image != null ? _context.Images.FirstOrDefault(i => i.Id == incomingDTO.Image.Id) : new Models.HomeInv.Image()
                {
                    Id = 0,
                    Data = incomingDTO.Image != null ? incomingDTO.Image.Data : Array.Empty<byte>(),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Set complex relations.
                productDB.Suppliers = incomingDTO.Suppliers != null ? incomingDTO.Suppliers.Select(s => _context.Suppliers.FirstOrDefault(sup => sup.Id == s.SupplierId)).ToList() : new List<Models.HomeInv.Supplier>();
                productDB.Tags = incomingDTO.Tags != null ? incomingDTO.Tags.Select(t => _context.Tags.FirstOrDefault(tag => tag.Id == t.TagId)).ToList() : new List<Models.HomeInv.Tag>();
                productDB.Countries = incomingDTO.OriginCountries != null ? incomingDTO.OriginCountries.Select(c => _context.Countries.FirstOrDefault(country => country.Id == c.CountryId)).ToList() : new List<Models.HomeInv.Country>();

                await _context.SaveChangesAsync();

                var outgoingDTO = MapDBModelToOutgoingDTO(productDB);

                return outgoingDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private UpdateProductDetailsOutDTO MapDBModelToOutgoingDTO(Models.HomeInv.Product productDB)
        {
            try
            {
                UpdateProductDetailsOutDTO outgoingDTO = new UpdateProductDetailsOutDTO();

                outgoingDTO.ProductId = productDB.Id;
                outgoingDTO.ProductName = productDB.Name;
                outgoingDTO.EanCode = productDB.Barcode;
                outgoingDTO.Brand = productDB.Brand;
                outgoingDTO.ExpirationDate = productDB.ExpiresAt;
                outgoingDTO.UpdatedAt = productDB.UpdatedAt;
                outgoingDTO.Category = new ProductCategoryDTO
                {
                    CategoryId = productDB.Category.Id,
                    CategoryName = productDB.Category.Name
                };
                outgoingDTO.Image = new ProductImageDTO
                {
                    Id = productDB.Image.Id,
                    Data = productDB.Image.Data
                };
                outgoingDTO.Locations = productDB.ProductLocations != null ? productDB.ProductLocations.Select(pl => new ProductLocationDTO
                {
                    LocationId = pl.Location.Id,
                    LocationName = pl.Location.Name,
                    Quantity = pl.Amount
                }).ToList() : new List<ProductLocationDTO>();
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}