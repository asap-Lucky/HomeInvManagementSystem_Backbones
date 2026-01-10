using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Outbound
{
    public class UpdateProductDetailsOutDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public string? EanCode { get; set; }

        public string? Brand { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ProductCategoryDTO Category { get; set; } = null!;

        public ProductImageDTO? Image { get; set; }

        public List<ProductLocationDTO>? Locations { get; set; }

        // NOTE: One product can be produced in multiple countries. Example: Nuts collected in Bolivia, packaged for Lidl in Germany.
        public List<ProductCountryDTO>? OriginCountries { get; set; }

        public List<ProductSupplierDTO>? Suppliers { get; set; }

        public List<ProductTagDTO>? Tags { get; set; }
    }
}
