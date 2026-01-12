using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Image
{
    public class ImageUploadInDTO
    {
        public byte[] ImageBytes { get; set; } = null!;

        public string Extension { get; set; } = null!;
    }
}
