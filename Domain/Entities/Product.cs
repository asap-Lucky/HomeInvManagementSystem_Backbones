using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Category { get; set; }
        public List<int> Locations { get; set; }
        public string? EANCode { get; set; }
        public string? Brand { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public List<string>? CountriesOfOrigin { get; set; }
        public List<string>? Suppliers { get; set; }
        public object? ImageBLOB { get; set; }
        public List<string>? Tags { get; set; }
    }
}
