using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Commands
{
    public interface IInventoryCommand
    {
        public Task<CreateProductOutDTO> AddProductToInventoryAsync(CreateProductInDTO createProdInDTO);

        public Task AddMockProductsAsync(int mockAmmount);

        public Task<UpdateProductDetailsOutDTO> UpdateProductDetailsAsync(UpdateProductDetailsInDTO updateProdInDTO);
    }
}
