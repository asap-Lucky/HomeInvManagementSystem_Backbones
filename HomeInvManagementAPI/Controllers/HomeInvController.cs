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
                    location = Domain.Enums.ProductLocation.Unassigned.ToString();

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
        public async Task<ActionResult<CreateProductResponse>> CreateProductInInventoryAsync([FromBody] CreateProductRequest request)
        {
            try
            {
                // TODO: Imp base request wrapper later to set some values that are needed later.
                //BaseRequestWrapper<CreateProductRequest> requestWrapper = new()
                //{
                //};

                CreateProductInDTO inDTO = new()
                {
                    ProductName = request.ProductName,
                    CategoryId = (int)request.Category,
                    Locations = request.Locations
                                              .Select(location => (int)location)
                                              .ToList(),
                    EANCode = request.EANCode,
                    Brand = request.Brand,
                    OriginCountries = request.OriginCountries,
                    Suppliers = request.Suppliers,
                    Tags = request.Tags
                };

                var outDTO = await _inventoryCommand.AddProductToInventoryAsync(inDTO);

                CreateProductResponse response = new()
                {
                    ProductId = outDTO.ProductId,
                    ProductName = outDTO.ProductName,
                    EanCode = outDTO.EanCode,
                    Brand = outDTO.Brand,
                    CreatedAt = outDTO.CreatedAt,
                    UpdatedAt = outDTO.UpdatedAt,
                    Category = outDTO.Category,
                    ImageId = outDTO.ImageId,
                    Locations = outDTO?.Locations,
                    OriginCountries = outDTO?.CountriesOfOrigin,
                    Suppliers = outDTO?.Suppliers,
                    Tags = outDTO?.Tags
                };

                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new inventory item.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut("{productId}")]
        [ProducesResponseType<UpdateProductDetailsResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdateProductDetailsResponse>> UpdateProductInfoAsync([FromRoute] int productId, [FromBody] UpdateProductDetailsRequest request)
        {
            try
            {
                if (productId < 0)
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid product id. Make sure the product id is filled out and is valid id bigger than 0");

                UpdateProductDetailsInDTO inDTO = new()
                {
                    ProductId = productId,
                    ProductName = request.ProductName,
                    EanCode = request.EanCode,
                    Brand = request.Brand,
                    CategoryId = request.CategoryId,
                    ImageId = request.ImageId,
                    OriginCountries = request.OriginCountries,
                    Suppliers = request.Suppliers,
                    Tags = request.Tags
                };

                var outDTO = await _inventoryCommand.UpdateProductDetailsAsync(inDTO);

                UpdateProductDetailsResponse response = new()
                {
                    ProductId = outDTO.ProductId,
                    ProductName = outDTO.ProductName,
                    EanCode = outDTO.EanCode,
                    Brand = outDTO.Brand,
                    UpdatedAt = outDTO.UpdatedAt,
                    Category = outDTO.Category,
                    ImageId = outDTO.ImageId,
                    OriginCountries = outDTO.OriginCountries,
                    Suppliers = outDTO.Suppliers,
                    Tags = outDTO.Tags
                };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product details.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPatch("{location}/{productId}")]
        [ProducesResponseType<UpdateLocationStockResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdateLocationStockResponse>> UpdateStockQuantityOnLocationAsync([FromRoute] int productId, [FromRoute] string location, [FromBody] UpdateLocationStockRequest request)
        {
            try
            {
                if (productId < 0)
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid product id. Make sure the product id is filled out and is valid id bigger than 0");

                if (!Enum.TryParse<Domain.Enums.ProductLocation>(location, true, out Domain.Enums.ProductLocation locationEnum))
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid location. Please define a valid location");

                UpdateLocationStockInDTO inDTO = new()
                {
                    ProductId = productId,
                    LocationId = (int)locationEnum,
                    Delta = request.Delta
                };

                var outDTO = await _inventoryCommand.UpdateLocationStockAsync(inDTO);

                UpdateLocationStockResponse response = new()
                {
                    ProductId = outDTO.ProductId,
                    Location = outDTO.Location,
                    Quantity = outDTO.Quantity,
                };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating stock quantity at location.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpDelete("product")]
        [ProducesResponseType<UpdateLocationStockResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DeleteProductResponse>> DeleteProductInInventoryAsync([FromQuery] int id)
        {
            try
            {
                if (id < 0)
                    return StatusCode(StatusCodes.Status400BadRequest, "Invalid product id. Make sure the product id is filled out and is valid id bigger than 0");

                DeleteProductInDTO inDTO = new()
                {
                    ProductId = id
                };

                var outDTO = await _inventoryCommand.DeleteProductFromInventoryAsync(inDTO);

                DeleteProductResponse response = new()
                {
                    ProductId = outDTO.ProductId,
                    ProductName = outDTO.ProductName,
                    EanCode = outDTO.EanCode,
                    Brand = outDTO.Brand,
                    CreatedAt = outDTO.CreatedAt,
                    UpdatedAt = outDTO.UpdatedAt,
                    Category = outDTO.Category,
                    ImageId = outDTO.ImageId,
                    Locations = outDTO.Locations,
                    OriginCountries = outDTO.OriginCountries,
                    Suppliers = outDTO.Suppliers,
                    Tags = outDTO.Tags
                };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erorr occured while trying to delete product");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        #region MockEndpoints
        [HttpPost("mock/{amount}")]
        public async Task<ActionResult> AddMockProductsAsync([FromRoute] int amount)
        {
            try
            {
                await _inventoryCommand.AddMockProductsAsync(amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }
        #endregion

        #region Batch Endpoints
        [HttpPatch("batch/{location}")]
        [ProducesResponseType<UpdateLocationStockResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BatchUpdateLocationStockResponse>> BatchUpdateStockAtLocationsAsync([FromRoute] string location, [FromBody] List<BatchUpdateLocationStockRequest> batchUpdateRequest)
        {
            try
            {
                if (!Enum.TryParse<Domain.Enums.ProductLocation>(location, true, out Domain.Enums.ProductLocation locationEnum))
                    return BadRequest("Invalid location. Please define a valid location");

                BatchUpdateLocationStockInDTO inDTO = new()
                {
                    LocationId = (int)locationEnum,
                    StockDeltas = batchUpdateRequest.Select(item => new StockDeltaItemInDTO
                    {
                        ProductId = item.ProductId,
                        Delta = item.Delta
                    }).ToList()
                };

                var outDTO = await _inventoryCommand.BatchUpdateLocationStockAsync(inDTO);

                BatchUpdateLocationStockResponse response = new()
                {
                    Location = outDTO.Location,
                    UpdatedStocks = outDTO.StockDeltas.Select(item => new StockDeltaItemOutDTO
                    {
                        ProductId = item.ProductId,
                        Delta = item.Delta,
                        OpeningStock = item.OpeningStock,
                        ClosingStock = item.ClosingStock,
                    }).ToList(),
                    TransactionId = outDTO.TransactionId
                };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while batch updating stock at location.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
        #endregion
    }
}