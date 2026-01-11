using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.ImageHandler
{
    public interface IImageUploadRepository
    {
        public Task<int> UploadImageAsync(byte[] imageData);
    }
}
