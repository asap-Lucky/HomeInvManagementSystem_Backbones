using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Image
{
    public class ImageUploadOutDTO
    {
        public int ImageId { get; set; }
        public bool IsUploaded { get; set; }
        public string? URL { get; set; }
        public bool IsAssigned { get; set; }
    }
}
