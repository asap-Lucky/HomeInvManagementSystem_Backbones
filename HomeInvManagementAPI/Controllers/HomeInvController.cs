using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Domain.Enums;
using HomeInvManagementAPI.DTOs.Request;
using HomeInvManagementAPI.DTOs.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace HomeInvManagementAPI.Controllers
{
    // NOTE: Make sure these requests can take both an ean and a product id due to the fact that the ean can be null (apple, banana, pear etc.) dont have an ean code.

    [Route("[controller]/v1")]
    [ApiController]
    public class HomeInvController : Controller
    {
        // Injections
        private readonly IInventoryCommand _inventoryCommand;
        private readonly IInventoryQuery _inventoryQuery;
        private readonly ILogger<HomeInvController> _logger;

        public HomeInvController(IInventoryQuery inventoryQuery, IInventoryCommand inventoryCommand, ILogger<HomeInvController> logger)
        {
            _inventoryCommand = inventoryCommand;
            _inventoryQuery = inventoryQuery;
            _logger = logger;
        }

        [HttpGet("products/{location}/{eancode}")]
        public async Task<ActionResult<ProductAggregateDTO>> GetProductDetailsByEanAsync([FromRoute] string location, [FromRoute] string eancode, [FromQuery] string source = "auto")
        {
            try
            {
                string trimmedEanCode = eancode.Replace(" ", "").Trim();

                // Check for EanCode lenght (8 - 13 characters)
                if (trimmedEanCode.Length < 8 || trimmedEanCode.Length > 13)
                    return BadRequest("Barcode does not match the lenght of a EAN barcode. Must be between 8 - 13 characters long");

                if (!trimmedEanCode.All(char.IsDigit))
                    return BadRequest("Invalid EAN code format. EAN code must contain only numeric characters.");

                // Check for valid route parameters (location and source)
                if (!Enum.TryParse<ProductLocation>(location, true, out ProductLocation locationEnum))
                    return BadRequest("Invalid location. Please define a valid location");

                if (!Enum.TryParse<SourceDestination>(source, true, out SourceDestination sourceEnum))
                    return BadRequest("Invalid source. Please define a valid source");

                ProductAggregateDTO productResponse = await _inventoryQuery.GetProductByEanAsync(trimmedEanCode, locationEnum, sourceEnum);

                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("products/{location}")]
        public async Task<ActionResult<List<ProductAggregateDTO>>> GetAllProductsAsync([FromRoute] string location)
        {
            try
            {
                // Check for valid route parameters (location and source)
                if (!Enum.TryParse<ProductLocation>(location.Trim().Replace(" ", ""), true, out ProductLocation locationEnum))
                    return BadRequest("Invalid location. Please define a valid location");

                List<ProductAggregateDTO> productListResponse = await _inventoryQuery.GetProductsFromInventoryAsync(locationEnum);

                return Ok(productListResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("inventory")]
        [ProducesResponseType<CreateProductResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateInventoryItemAsync([FromBody] CreateProductRequest productRequest)
        {
            try
            {
                // TODO: Imp base request wrapper later to set some values that are needed later.
                //BaseRequestWrapper<CreateProductRequest> requestWrapper = new()
                //{
                //};

                CreateProductInDTO createItemDTO = new()
                {
                    ProductName = productRequest.ProductName,
                    Category = (int)productRequest.Category,
                    Locations = productRequest.Locations
                                              .Select(location => (int)location)
                                              .ToList(),
                    EANCode = productRequest.EANCode,
                    Brand = productRequest.Brand,
                    ExpirationDate = productRequest.ExpirationDate,
                    CountriesOfOrigin = productRequest.CountriesOfOrigin,
                    Suppliers = productRequest.Suppliers,
                    ImageBLOB = productRequest.ImageBLOB, 
                    Tags = productRequest.Tags
                };

                CreateProductOutDTO addedItemDTO = await _inventoryCommand.AddProductToInventoryAsync(createItemDTO);

                    if (addedItemDTO == null)
                    return new StatusCodeResult(StatusCodes.Status422UnprocessableEntity);

                CreateProductResponse productResponse = new()
                {
                    ProductId = addedItemDTO.ProductId,
                    ProductName = addedItemDTO.ProductName,
                    EanCode = addedItemDTO.EanCode,
                    Brand = addedItemDTO.Brand,
                    ExpirationDate = addedItemDTO.ExpirationDate,
                    CreatedAt = addedItemDTO.CreatedAt,
                    UpdatedAt = addedItemDTO.UpdatedAt,
                    Category = addedItemDTO.Category,
                    ImageBLOB = addedItemDTO.ImageBLOB,
                    Locations = addedItemDTO?.Locations?.Select(x => x.LocationName).ToList() ?? null,
                    CountriesOfOrigin = addedItemDTO?.CountriesOfOrigin?.Select(x => x.CountryName).ToList() ?? null,
                    Suppliers = addedItemDTO?.Suppliers?.Select(x => x.SupplierName).ToList() ?? null,
                    Tags = addedItemDTO?.Tags?.Select(x => x.TagName).ToList() ?? null
                };

                return StatusCode(StatusCodes.Status201Created, productResponse);
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