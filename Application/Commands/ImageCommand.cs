using Application.Interfaces.Commands;
using Application.Interfaces.Repositories.ImageHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public class ImageCommand : IImageCommand
    {
        // Injections 
        private readonly IImageUploadRepository _imageUploadRepo;


        public ImageCommand(IImageUploadRepository uploadRepository)
        {
            _imageUploadRepo = uploadRepository;
        }

        public async Task<int> UploadImageAsync(byte[] imageBytes)
        {
            try
            {
                int imageId = await _imageUploadRepo.UploadImageAsync(imageBytes);
                return imageId;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
