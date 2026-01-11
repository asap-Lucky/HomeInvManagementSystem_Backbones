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

        public async Task<Image> DownloadImageAsync(int imageId)
        {
            try
            {
                throw new NotImplementedException();
                //var imageData = await 
                //return imageData;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
