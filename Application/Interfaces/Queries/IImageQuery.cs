using Application.DTOs.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Queries
{
    public interface IImageQuery
    {
        public Task<ImageDownloadOutDTO> DownloadImageAsync(ImageDownloadInDTO inDTO);
    }
}
