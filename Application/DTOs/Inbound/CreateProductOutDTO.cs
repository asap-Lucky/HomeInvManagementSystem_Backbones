using Domain.Enums;
using Domain.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inbound
{
    public class CreateProductOutDTO
    {
        public required string ProductName { get; set; }
        public required int Category { get; set; }
        public required List<int> Locations { get; set; }
        public string? EANCode { get; set; }
        public string? Brand { get; set; }
        public string? ExpirationDate { get; set; }
        public List<string>? CountriesOfOrigin { get; set; }
        public List<string>? Suppliers { get; set; }
        public string? ImageBLOB { get; set; }
        public List<string>? Tags { get; set; }
    }
}
