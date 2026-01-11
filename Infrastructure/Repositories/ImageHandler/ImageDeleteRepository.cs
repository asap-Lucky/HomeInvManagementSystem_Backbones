using Application.Interfaces.Repositories.ImageHandler;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ImageHandler
{
    public class ImageDeleteRepository : IImageDeleteRepository
    {        
        // Injections
        private readonly HomeinvsystemContext _context;

        public ImageDeleteRepository(HomeinvsystemContext context)
        {
            _context = context;
        }
    }
}
