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
        private readonly IImageDeleteRepository _imageDeleteRepo;

        public ImageCommand(IImageUploadRepository uploadRepository, IImageDeleteRepository imageDeleteRepo)
        {
            _imageUploadRepo = uploadRepository;
            _imageDeleteRepo = imageDeleteRepo;
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

        public async Task<ImageDeleteOutDTO?> DeleteImageAsync(ImageDeleteInDTO inDTO)
        {
            try
            {
                ImageDeleteOutDTO? outDTO = await _imageDeleteRepo.DeleteImageAsync(inDTO);
                return outDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
