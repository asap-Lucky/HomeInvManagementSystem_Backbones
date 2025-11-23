using Microsoft.AspNetCore.Mvc;

namespace HomeInvManagementAPI.Controllers
{
    [ApiController]
    public class UnifiedProductService : Controller
    {
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
