using Microsoft.AspNetCore.Mvc;

namespace HomeInvManagementAPI.Controllers
{
    [Route("[controller]/v1")]
    [ApiController]
    public class OpenFoodFactsController : Controller
    {
        [HttpGet]
        public async Task<ActionResult> GetProductDetails(string eanCode)
        {
            try
            {
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
