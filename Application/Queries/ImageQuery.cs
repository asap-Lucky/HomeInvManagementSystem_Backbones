using Application.DTOs.Image;
using Application.Interfaces.Queries;
using Application.Interfaces.Repositories.ImageHandler;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class ImageQuery : IImageQuery
    {
        // Injections
        private readonly IImageDownloadRepository _imageDownloadRepo;

        public ImageQuery(IImageDownloadRepository imageDownloadRepository) 
        { 
            _imageDownloadRepo = imageDownloadRepository;
        }

        public async Task<ImageDownloadOutDTO> DownloadImageAsync(ImageDownloadInDTO inDTO)
        {
            try
            {
                var outDTO = await _imageDownloadRepo.DownloadImageAsync(inDTO);
                return outDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
