using Application.DTOs.Image;
using Application.Interfaces.Repositories.ImageHandler;
using Infrastructure.Data;
using Infrastructure.Models.HomeInv;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.ImageHandler
{
    public class ImageUploadRepository : IImageUploadRepository
    {
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<ImageUploadRepository> _logger;

        public ImageUploadRepository(HomeinvsystemContext context, ILogger<ImageUploadRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ImageUploadOutDTO> UploadImageAsync(ImageUploadInDTO inDTO)
        {
            try
            {
                var newImage = new Image
                {
                    Data = inDTO.ImageBytes,
                    CreatedAt = DateTime.Now,
                    Extension = inDTO.Extension
                };

                _context.Images.Add(newImage);
                await _context.SaveChangesAsync();

                var outDTO = new ImageUploadOutDTO
                {
                    ImageId = newImage.Id,
                    IsUploaded = true,
                    URL = null // TODO: Generate URL.
                };

                return outDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the image to the database.");
                throw;
            }
        }
    }
}
