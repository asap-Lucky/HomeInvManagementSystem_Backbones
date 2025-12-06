using Domain.Entities;
using Domain.Enums;
using Domain.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Application.DTOs.Inbound
{
    public class CreateProductOutDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string? EanCode { get; set; }

        public string? Brand { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int Category { get; set; }

        public List<int> Locations { get; set; }

        // NOTE: One product can be produced in multiple countries. Example: Nuts collected in Bolivia, packaged for Lidl in Germany.
        public List<string>? CountriesOfOrigin { get; set; }

        public List<string>? Suppliers { get; set; }

        public string? ImageBLOB { get; set; }

        public List<string>? Tags { get; set; }
    }
}
