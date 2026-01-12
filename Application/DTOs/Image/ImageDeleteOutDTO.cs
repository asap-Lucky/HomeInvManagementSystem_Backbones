using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Image
{
    public class ImageDeleteOutDTO
    {
        public int ImageId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
