using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Outbound
{
    public class CreateProductInDTO
    {
        public string ProductName { get; set; }
        public int Category { get; set; }
        public List<int> Locations { get; set; }
        public string? EANCode { get; set; }
        public string? Brand { get; set; }
        public string? ExpirationDate { get; set; }
        public List<string>? CountriesOfOrigin { get; set; }
        public List<string>? Suppliers { get; set; }
        public string? ImageBLOB { get; set; }
        public List<string>? Tags { get; set; }
    }
}
