using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Domain.Enums;
using HomeInvManagementAPI.DTOs.Request;
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
        private readonly IInventoryCommand _productAggregateCommand;

        private readonly IProductAggregateQuery _productAggregateQuery;
        private readonly ILogger<HomeInvController> _logger;

        public HomeInvController(IProductAggregateQuery productAggregateQuery, IInventoryCommand productAggregateCommand, ILogger<HomeInvController> logger)
        {
            _productAggregateCommand = productAggregateCommand;
            _productAggregateQuery = productAggregateQuery;
            _logger = logger;
        }

        [HttpGet("products/{location}/{eancode}")]
        public async Task<ActionResult<ProductAggregateDTO>> GetProductDetailsByEanAsync([FromRoute] ProductLocation location, [FromRoute] string eancode, [FromQuery] SourceDestination source = SourceDestination.Auto)
        {
            try
            {
                string trimmedEanCode = eancode.Replace(" ", "").Trim();

                // Check for EanCode lenght (8 - 13 characters)
                if (trimmedEanCode.Length < 8 || trimmedEanCode.Length > 13)
                    return BadRequest("Barcode does not match the lenght of a EAN barcode. Must be between 8 - 13 characters long");

                if (!trimmedEanCode.All(char.IsDigit))
                    return BadRequest("Invalid EAN code format. EAN code must contain only numeric characters.");

                ProductAggregateDTO productResponse = await _productAggregateQuery.GetProductByEanAsync(trimmedEanCode, source, location);

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

        [HttpPost("inventory")]                                             
        public async Task<ActionResult> CreateInventoryItemAsync([FromBody] CreateProductRequest productRequest)
        {
            try
            {
                // Are we sure we dont mix request/response and command/dto here? Try to figure out if cleaner way exists.
                return Ok();
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
                // NOTE: This will hit the Stored Procedure for getting all the products in the database. Since its a big query to handle otherwise.
                return Ok();
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
                // NOTE: This will hit the Stored Procedure for getting all the products in the database. Since its a big query to handle otherwise.
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        // BULK OPERATIONS (Using Stored procedures)
    }
}