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

        public async Task<int> UploadImageAsync(byte[] imageData)
        {
            try
            {
                var newImage = new Image
                {
                    Data = imageData,
                    CreatedAt = DateTime.Now
                };

                _context.Images.Add(newImage);
                await _context.SaveChangesAsync();

                return newImage.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the image to the database.");
                throw;
            }
        }
    }
}
