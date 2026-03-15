using Application.DTOs;
using Application.DTOs.Inbound;
using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Azure;
using Domain.Common.Exceptions;
using Domain.Common.Rules;
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
    /// <summary>
    /// TODO:
    /// - Make generic wrapper, which wraps the returning values in an error and body wrapping for each call.
    /// </summary>
    [Route("[controller]/v1")]
    [ApiController]
    public class HomeInvController : Controller
    {
        // Injections
        private readonly IInventoryCommand _inventoryCommand;
        private readonly ILocationQuery _locationQuery;
        private readonly IProductQuery _inventoryQuery;
        private readonly ILogger<HomeInvController> _logger;

        public HomeInvController(IProductQuery inventoryQuery, IInventoryCommand inventoryCommand, ILocationQuery locationQuery, ILogger<HomeInvController> logger)
        {
            _inventoryCommand = inventoryCommand;
            _inventoryQuery = inventoryQuery;
            _locationQuery = locationQuery;
            _logger = logger;
        }

        [HttpGet("products/{id}")]
        [ProducesResponseType<GetProductOutDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetProductOutDTO>> GetProductById([FromRoute] string id)
        {
            try
            {
                // Validation of id.
                var validId = new ProductId(id);

                string productId = validId.Value;

                GetProductOutDTO? outDTO = await _inventoryQuery.GetProductByIdAsync(productId);

                if (outDTO == null)
                    return StatusCode(StatusCodes.Status404NotFound); 

                GetProductResponse response = new()
                {
                    ProductId = outDTO.ProductId,
                    ProductName = outDTO.ProductName,
                    Barcode = outDTO.Barcode,
                    Brand = outDTO.Brand,
                    CreatedAt = outDTO.CreatedAt,
                    UpdatedAt = outDTO.UpdatedAt,
                    Category = outDTO.Category,
                    ImageId = outDTO.ImageId,
                    Locations = outDTO.Locations,
                    CountriesOfOrigin = outDTO.OriginCountries,
                    Suppliers = outDTO.Suppliers,
                    Tags = outDTO.Tags
                };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (DomainRuleViolationException ex)
            {
                _logger.LogError(ex, "Domain rule violation occurred while fetching products from inventory.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching product by ID. Id Value: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("products/by-barcode/{barcode}")]
        [ProducesResponseType<GetProductOutDTO>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GetProductOutDTO>> GetProductByBarcode([FromRoute] string barcode)
        {
            try
            {
                // Validate barcode.
                var validBarcode = new ProductBarcode(barcode);

                string barcodeValue = validBarcode.Value;

                GetProductOutDTO? outDTO = await _inventoryQuery.GetProductByBarcodeAsync(barcodeValue);

                if (outDTO == null)
                    return StatusCode(StatusCodes.Status404NotFound);

                GetProductResponse response = new()
                {
                    ProductId = outDTO.ProductId,
                    ProductName = outDTO.ProductName,
                    Barcode = outDTO.Barcode,
                    Brand = outDTO.Brand,
                    CreatedAt = outDTO.CreatedAt,
                    UpdatedAt = outDTO.UpdatedAt,
                    Category = outDTO.Category,
                    ImageId = outDTO.ImageId,
                    Locations = outDTO.Locations,
                    CountriesOfOrigin = outDTO.OriginCountries,
                    Suppliers = outDTO.Suppliers,
                    Tags = outDTO.Tags
                };

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (DomainRuleViolationException ex)
            {
                _logger.LogError(ex, "Domain rule violation occurred while fetching products from inventory.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching product by barcode. Barcode value {barcode}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("locations/{locationId}/products")]
        [ProducesResponseType<List<GetProductOutDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetProductOutDTO>>> GetProductsOnLocation([FromRoute] string locationId)
        {
            try
            {
                // Validate locationId.
                var validLocationId = new LocationId(locationId);

                string locationIdValue = validLocationId.Value;

                List<GetProductLocationOutDTO>? outDTOs = await _inventoryQuery.GetProductsByLocationIdAsync(locationIdValue);

                if (outDTOs == null)
                    return StatusCode(StatusCodes.Status404NotFound);

                List<GetProductLocationResponse> response = outDTOs.Select(outDTO => new GetProductLocationResponse
                {
                    ProductId = outDTO.ProductId,
                    ProductName = outDTO.ProductName,
                    Stock = outDTO.Stock
                }).ToList();

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (DomainRuleViolationException ex)
            {
                _logger.LogError(ex, "Domain rule violation occurred while fetching products from inventory.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching products from inventory.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("locations")]
        [ProducesResponseType<List<GetProductOutDTO>>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<GetProductOutDTO>>> GetLocations()
        {
            try
            {
                List<LocationDTO>? outDTOs = await _locationQuery.GetAllLocationsAsync();

                if (outDTOs == null)
                    return StatusCode(StatusCodes.Status404NotFound);
                
                List<GetLocationResponse> response = outDTOs.Select(outDTO => new GetLocationResponse
                {
                    Id = outDTO.Id,
                    Name = outDTO.Name
                }).ToList();

                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching locations from inventory.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("products")]
        [ProducesResponseType<CreateProductResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CreateProductResponse>> AddProductToInventory([FromBody] CreateProductRequest request)
        {
            try
            {
                // Validation of request parameters.
                ProductName productName = new(request.ProductName);
                ProductBarcode? barcode = request.Barcode != null ? new ProductBarcode(request.Barcode) : null;

                CreateProductInDTO inDTO = new()
                {
                    ProductName = productName.Value,
                    CategoryId = request.CategoryId,
                    Locations = request.Locations,
                    Barcode = barcode?.Value,
                    Brand = request.Brand,
                    OriginCountries = request.OriginCountries,
                    Suppliers = request.Suppliers,
                    Tags = request.Tags,
                    ImageId = request.ImageId
                };

                var outDTO = await _inventoryCommand.CreateProduct(inDTO);

                CreateProductResponse response = new()
                {
                    ProductId = outDTO.ProductId,
                    ProductName = outDTO.ProductName,
                    Barcode = outDTO.EanCode,
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
            catch (DomainRuleViolationException ex)
            {
                _logger.LogError(ex, "Domain rule violation occurred while fetching products from inventory.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new inventory item.");
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut("products/{id}")]
        [ProducesResponseType<UpdateProductDetailsResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UpdateProductDetailsResponse>> UpdateProductInfoAsync([FromRoute] int id, [FromBody] UpdateProductDetailsRequest request)
        {
            try
            {
                // Validation of request parameters.
                ProductName productName = new(request.ProductName);
                ProductBarcode? barcode = request.Barcode != null ? new ProductBarcode(request.Barcode) : null;
                ProductId productId = new(id.ToString());

                UpdateProductDetailsInDTO inDTO = new()
                {
                    ProductName = productName.Value,
                    Barcode = barcode?.Value,
                    Brand = request.Brand,
                    CategoryId = request.CategoryId,
                    ImageId = request.ImageId,
                    OriginCountries = request.OriginCountries,
                    Suppliers = request.Suppliers,
                    Tags = request.Tags
                };
                
                var outDTO = await _inventoryCommand.UpdateProductDetailsAsync(productId.Value, inDTO);

                if (outDTO == null)
                    return StatusCode(StatusCodes.Status404NotFound, "Product not found.");

                UpdateProductDetailsResponse response = new()
                {
                    ProductId = outDTO.ProductId,
                    ProductName = outDTO.ProductName,
                    Barcode = outDTO.Barcode,
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
            catch (DomainRuleViolationException ex)
            {
                _logger.LogError(ex, "Domain rule violation occurred while fetching products from inventory.");
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
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

                //if (!Enum.TryParse<Domain.Enums.ProductLocation>(location, true, out Domain.Enums.ProductLocation locationEnum))
                //    return StatusCode(StatusCodes.Status400BadRequest, "Invalid location. Please define a valid location");

                UpdateLocationStockInDTO inDTO = new()
                {
                    ProductId = productId,
                    //LocationId = (int)locationEnum,
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
                //if (!Enum.TryParse<Domain.Enums.ProductLocation>(location, true, out Domain.Enums.ProductLocation locationEnum))
                //    return BadRequest("Invalid location. Please define a valid location");

                BatchUpdateLocationStockInDTO inDTO = new()
                {
                    //LocationId = (int)locationEnum,
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