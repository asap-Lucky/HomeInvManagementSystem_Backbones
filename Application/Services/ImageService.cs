using Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    internal class ImageService : IImageService
    {
        public string SerializeImageToBase64(byte[] imageBytes)
        {
            return Convert.ToBase64String(imageBytes);
        }

        public byte[] DeserializeBase64ToImage(string base64String)
        {
            return Convert.FromBase64String(base64String);
        }
    }
}
