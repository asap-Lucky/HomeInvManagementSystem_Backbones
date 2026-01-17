using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Azure;
using Domain.Enums;
using HomeInvManagementAPI.DTOs.Request;
using HomeInvManagementAPI.DTOs.Response;
using Infrastructure.Models.HomeInv;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace HomeInvManagementAPI.Controllers
{
    // NOTE: Make sure these requests can take both an ean and a product id due to the fact that the ean can be null (apple, banana, pear etc.) dont have an ean code.

    [Route("[controller]/v1/products")]
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

        [HttpGet("{location}/{eancode}")]
        [ProducesResponseType<ProductAggregateDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductAggregateDTO>> GetProductByEanAsync([FromRoute] string location, [FromRoute] string eancode, [FromQuery] string source = "auto")
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
                if (!Enum.TryParse<Domain.Enums.ProductLocation>(location, true, out Domain.Enums.ProductLocation locationEnum))
                    return BadRequest("Invalid location. Please define a valid location");

                if (!Enum.TryParse<SourceDestination>(source, true, out SourceDestination sourceEnum))
                    return BadRequest("Invalid source. Please define a valid source");

                ProductAggregateDTO productResponse = await _inventoryQuery.GetProductByEanAsync(trimmedEanCode, locationEnum, sourceEnum);

                return StatusCode(StatusCodes.Status200OK, productResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product by EAN.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("{location}")]
        [ProducesResponseType<ProductAggregateDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest),]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<ProductAggregateDTO>>> GetProductsOnLocationAsync([FromRoute] string location = " ", [FromQuery] bool allLocations = false)
        {
            try
            {
                if (allLocations)
                {
                    location = Domain.Enums.ProductLocation.Unassigned.ToString();
                }

                // Check for valid route parameters (location and source)
                if (!Enum.TryParse<Domain.Enums.ProductLocation>(location.Trim().Replace(" ", ""), true, out Domain.Enums.ProductLocation locationEnum))
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid location. Please define a valid location");

                List<ProductAggregateDTO> productListResponse = await _inventoryQuery.GetProductsFromInventoryAsync(locationEnum, allLocations);

                return StatusCode(StatusCodes.Status200OK, productListResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products from inventory.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        [ProducesResponseType<CreateProductResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateProductResponse>> CreateProductInInventoryAsync([FromBody] CreateProductRequest productCreateRequest)
        {
            try
            {
                _inventoryCommand.AddMockProductsAsync(10);
                return Ok();
                // TODO: Imp base request wrapper later to set some values that are needed later.
                //BaseRequestWrapper<CreateProductRequest> requestWrapper = new()
                //{
                //};

                CreateProductInDTO createItemDTO = new()
                {
                    ProductName = productCreateRequest.ProductName,
                    Category = (int)productCreateRequest.Category,
                    Locations = productCreateRequest.Locations
                                              .Select(location => (int)location)
                                              .ToList(),
                    EANCode = productCreateRequest.EANCode,
                    Brand = productCreateRequest.Brand,
                    ExpirationDate = productCreateRequest.ExpirationDate,
                    OriginCountries = productCreateRequest.OriginCountries,
                    Suppliers = productCreateRequest.Suppliers,
                    Tags = productCreateRequest.Tags
                };

                CreateProductOutDTO addedItemDTO = await _inventoryCommand.AddProductToInventoryAsync(createItemDTO);

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
                    ImageId = addedItemDTO.ImageId,
                    Locations = addedItemDTO?.Locations,
                    OriginCountries = addedItemDTO?.CountriesOfOrigin,
                    Suppliers = addedItemDTO?.Suppliers,
                    Tags = addedItemDTO?.Tags
                };

                return StatusCode(StatusCodes.Status201Created, productResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new inventory item.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut("{productid}")]
        [ProducesResponseType<UpdateProductDetailsResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UpdateProductDetailsResponse>> UpdateProductInfoAsync([FromRoute] int productId, [FromRoute] string location, [FromBody] UpdateProductDetailsInDTO productUpdateRequest)
        {
            try
            {
                if (productId == null || productId < 0)
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid product id. Make sure the product id is filled out and is valid id bigger than 0");

                // TODO: Validate other properties if needed. Move this validation later to Domain.
                if (!Enum.TryParse<Domain.Enums.ProductLocation>(location, true, out Domain.Enums.ProductLocation locationEnum))
                    return BadRequest("Invalid location. Please define a valid location");
                    
                var updatedProductDTO = await _inventoryCommand.UpdateProductDetailsAsync(productUpdateRequest);

                return Ok(updatedProductDTO);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        // TODO: Read docs about PATCH method and implement it properly.
        [HttpPatch("{location}/{productid}")]
        [ProducesResponseType<ProductQuantityResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductQuantityResponse>> UpdateStockQuantityOnLocationAsync([FromRoute] int productId, [FromRoute] string location, [FromBody] JsonPatchDocument<UpdateInvQuantityRequest> quantityUpdateRequest)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        // Remember to handle cascading deletes in the database for related entities.
        [HttpDelete("{productid}")]
        public async Task<ActionResult<ProductAggregateDTO>> DeleteProductInInventoryAsync()
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
    }
}