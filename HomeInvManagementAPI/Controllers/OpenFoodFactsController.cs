using HomeInvManagementAPI.DTOs.OpenFoodFactsProduct.Outbound;
using HomeInvManagementAPI.Interfaces.OpenFoodFacts;
using Microsoft.AspNetCore.Mvc;

namespace HomeInvManagementAPI.Controllers
{
    // TODO: Imp. a better desc.
    /// <summary>
    /// API Controller for interacting with OpenFoodFacts services.
    /// </summary>

    [Route("v1/[controller]")]
    [ApiController]
    public class OpenFoodFactsController : Controller
    {
        // Injections
        private readonly IOpenFoodFactsService _openFoodFactsService;

        public OpenFoodFactsController(IOpenFoodFactsService openFoodFactsService)
        {
            _openFoodFactsService = openFoodFactsService;
        }

        [HttpGet("OpenFoodFact/products/{eanCode}")]
        public async Task<ActionResult<OpenFoodFactsResponseApiDto>> GetProductDetails([FromQuery] string eanCode)
        {
            try
            {
                string trimmedEanCode = eanCode.Replace(" ", "").Trim();

                // Check for EanCode lenght (8 - 13 characters)
                if (trimmedEanCode.Length >= 8 && trimmedEanCode.Length <= 13)
                    return BadRequest("Barcode does not match the lenght of a EAN barcode. Must be between 8 - 13 characters long");

                if (!long.TryParse(trimmedEanCode, out long formattedEanCode))
                    return BadRequest("Invalid EAN code format. EAN code must contain only numeric characters.");

                OpenFoodFactsResponseApiDto productResponse = await _openFoodFactsService.GetProductByEanCodeAsync(trimmedEanCode);
                
                return Ok(productResponse);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }
    }
}
