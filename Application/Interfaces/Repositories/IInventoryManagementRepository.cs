using Application.DTOs.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Repositories
{
    public interface IInventoryManagementRepository
    {
        public Task<CreateProductInDTO> AddProductToInventoryAsync(CreateProductInDTO createInDTO);

        public Task<List<CreateProductInDTO>> AddProductsBulkToInventoryAsync(List<CreateProductInDTO> createBulkInDTO);
    }
}