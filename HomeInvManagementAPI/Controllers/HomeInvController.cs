using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Domain.Enums;
using HomeInvManagementAPI.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace HomeInvManagementAPI.Controllers
{
    // NOTE: Make sure these requests can take both an ean and a product id due to the fact that the ean can be null (apple, banana, pear etc.) dont have an ean code.

    [Route("[controller]/v1")]
    [ApiController]
    public class HomeInvController : Controller
    {
        // Injections
        private readonly IProductAggregateCommand _productAggregateCommand;

        private readonly IProductAggregateQuery _productAggregateQuery;
        private readonly ILogger<HomeInvController> _logger;

        public HomeInvController(IProductAggregateQuery productAggregateQuery, IProductAggregateCommand productAggregateCommand, ILogger<HomeInvController> logger)
        {
            _productAggregateCommand = productAggregateCommand;
            _productAggregateQuery = productAggregateQuery;
            _logger = logger;
        }

        [HttpGet("products/{location}/{id}")]
        public async Task<ActionResult<ProductAggregateDTO>> GetProductDetailsByEanAsync([FromRoute] string eanCode, [FromQuery] SourceDestinations source = SourceDestinations.Auto)
        {
            try
            {
                string trimmedEanCode = eanCode.Replace(" ", "").Trim();

                // Check for EanCode lenght (8 - 13 characters)
                if (trimmedEanCode.Length < 8 || trimmedEanCode.Length > 13)
                    return BadRequest("Barcode does not match the lenght of a EAN barcode. Must be between 8 - 13 characters long");

                if (!long.TryParse(trimmedEanCode, out long formattedEanCode))
                    return BadRequest("Invalid EAN code format. EAN code must contain only numeric characters.");

                ProductAggregateDTO productResponse = await _productAggregateQuery.GetProductByEanAsync(trimmedEanCode, source);

                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("products/{location}")]
        public async Task<ActionResult<List<ProductAggregateDTO>>> GetAllProductsAsync()
        {
            try
            {
                // NOTE: This will hit the Stored Procedure for getting all the products in the database. Since its a big query to handle otherwise.
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("inventory/{location}")]
        public async Task<ActionResult<ProductAggregateDTO>> CreateInventoryItemAsync()
        {
            try
            {
                string trimmedEanCode = eanCode.Replace(" ", "").Trim();

                // Check for EanCode lenght (8 - 13 characters)
                if (trimmedEanCode.Length < 8 || trimmedEanCode.Length > 13)
                    return BadRequest("Barcode does not match the lenght of a EAN barcode. Must be between 8 - 13 characters long");

                if (!long.TryParse(trimmedEanCode, out long formattedEanCode))
                    return BadRequest("Invalid EAN code format. EAN code must contain only numeric characters.");

                ProductAggregateDTO productResponse = await _productAggregatorService.GetProductByEanAsync(trimmedEanCode);

                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("inventory/{location}")]
        public async Task<ActionResult<ProductAggregateDTO>> UpdateInventoryItemAsync()
        {
            try
            {
                string trimmedEanCode = eanCode.Replace(" ", "").Trim();

                // Check for EanCode lenght (8 - 13 characters)
                if (trimmedEanCode.Length < 8 || trimmedEanCode.Length > 13)
                    return BadRequest("Barcode does not match the lenght of a EAN barcode. Must be between 8 - 13 characters long");

                if (!long.TryParse(trimmedEanCode, out long formattedEanCode))
                    return BadRequest("Invalid EAN code format. EAN code must contain only numeric characters.");

                ProductAggregateDTO productResponse = await _productAggregatorService.GetProductByEanAsync(trimmedEanCode);

                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("inventory/{location}/{id}")]
        public async Task<ActionResult<ProductAggregateDTO>> DeleteInventoryItemAsync()
        {
            try
            {
                string trimmedEanCode = eanCode.Replace(" ", "").Trim();

                // Check for EanCode lenght (8 - 13 characters)
                if (trimmedEanCode.Length < 8 || trimmedEanCode.Length > 13)
                    return BadRequest("Barcode does not match the lenght of a EAN barcode. Must be between 8 - 13 characters long");

                if (!long.TryParse(trimmedEanCode, out long formattedEanCode))
                    return BadRequest("Invalid EAN code format. EAN code must contain only numeric characters.");

                ProductAggregateDTO productResponse = await _productAggregatorService.GetProductByEanAsync(trimmedEanCode);

                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }
    }
}