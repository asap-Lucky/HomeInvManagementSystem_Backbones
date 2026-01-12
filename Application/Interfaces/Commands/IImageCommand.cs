using Application.DTOs.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Commands
{
    public interface IImageCommand
    {
        public Task<ImageUploadOutDTO> UploadImageAsync(ImageUploadInDTO inDTO);

        public Task<bool> DeleteImageAsync(int imageId);
    }
}
