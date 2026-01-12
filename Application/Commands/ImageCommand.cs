using Application.DTOs.Image;
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

        public async Task<ImageUploadOutDTO> UploadImageAsync(ImageUploadInDTO inDTO)
        {
            try
            {
                ImageUploadOutDTO outDTO = await _imageUploadRepo.UploadImageAsync(inDTO);
                return outDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(int imageId)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
