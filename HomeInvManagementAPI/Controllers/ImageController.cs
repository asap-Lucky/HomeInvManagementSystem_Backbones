using Microsoft.AspNetCore.Mvc;

namespace HomeInvManagementAPI.Controllers
{
    [Route("[controller]/v1")]
    [ApiController]
    public class ImageController : Controller
    {
        [HttpPost("serialize")]
        public ActionResult<string> SerializeImage([FromBody] byte[] imageBytes)
        {
            try
            {
                string base64String = Convert.ToBase64String(imageBytes);
                return Ok(base64String);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("deserialize")]
        public ActionResult<byte[]> DeserializeImage([FromBody] string base64String)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);
                return Ok(imageBytes);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }
    }
}
