using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inbound
{
    public class UpdateProductDetailsInDTO
    {
        public string ProductName { get; set; } = null!;

        public string? Barcode { get; set; }    

        public string? Brand { get; set; }

        public int CategoryId { get; set; }

        public int? ImageId { get; set; }

        public List<int>? OriginCountries { get; set; }

        public List<int>? Suppliers { get; set; }

        public List<int>? Tags { get; set; }
    }
}