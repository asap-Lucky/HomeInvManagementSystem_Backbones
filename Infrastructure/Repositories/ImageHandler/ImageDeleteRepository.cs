using Application.DTOs.Image;
using Application.Interfaces.Repositories.ImageHandler;
using Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories.ImageHandler
{
    public class ImageDeleteRepository : IImageDeleteRepository
    {        
        // Injections
        private readonly HomeinvsystemContext _context;
        private readonly ILogger<ImageDeleteRepository> _logger;

        public ImageDeleteRepository(HomeinvsystemContext context, ILogger<ImageDeleteRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ImageDeleteOutDTO?> DeleteImageAsync(ImageDeleteInDTO inDTO)
        {
            try
            {
                var image = await _context.Images.FindAsync(inDTO.ImageId);

                if (image == null)
                    return null;

                _context.Images.Remove(image);
                await _context.SaveChangesAsync();



                return new ImageDeleteOutDTO
                {
                    ImageId = inDTO.ImageId,
                    IsDeleted = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the image from the database.");
                throw;
            }
        }
    }
}
