using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories.InventoryManagement
{
    public interface ILocationReadRepository
    {
        Task<ProductLocationDTO> GetLocationByIdAsync(int id);
    }
}
