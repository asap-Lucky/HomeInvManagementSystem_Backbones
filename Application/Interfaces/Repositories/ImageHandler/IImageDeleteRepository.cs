using Application.DTOs.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.ImageHandler
{
    public interface IImageDeleteRepository
    {
        public Task<ImageDeleteOutDTO?> DeleteImageAsync(ImageDeleteInDTO inDTO);
    }
}
