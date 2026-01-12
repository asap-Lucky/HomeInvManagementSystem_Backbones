using Application.DTOs.Image;
using Application.Interfaces.Repositories.ImageHandler;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ImageHandler
{
    public class ImageDownloadRepository : IImageDownloadRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<ImageDownloadRepository> _logger;

        public ImageDownloadRepository(HomeinvsystemContext context)
        {
            _context = context;
        }

        public async Task<ImageDownloadOutDTO> DownloadImageAsync(ImageDownloadInDTO inDTO)
        {
            try
            {
                var image = await _context.Images.FindAsync(inDTO.ImageId);
                
                if (image == null)
                    throw new Exception("Image not found.");
                
                var outDTO = new ImageDownloadOutDTO
                {
                    ImageBytes = image.Data,
                    Extension = image.Extension
                };

                return outDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading the image from the database.");
                throw;
            }
        }
    }
}
