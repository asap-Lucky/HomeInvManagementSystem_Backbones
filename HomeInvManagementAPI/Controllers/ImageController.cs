using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IO;

namespace HomeInvManagementAPI.Controllers
{
    [Route("[controller]/v1")]
    [ApiController]
    public class ImageController : Controller
    {
        [HttpPost("serialize")]
        public ActionResult<string> SerializeImage(IFormFile image)
        {
            try
            {
                // Validation
                List<string> permittedExtensions = new() { ".jpg", ".jpeg", ".png" };
                var fileSizeLimit = 5 * 1024 * 1024; // 5 MB
                
                var imageExt = Path.GetExtension(image.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(imageExt) || !permittedExtensions.Contains(imageExt))
                {
                    return BadRequest("Invalid image format. Only .jpg, .jpeg, and .png are allowed.");
                }

                if (image.Length > fileSizeLimit)
                {
                    return BadRequest("File size exceeds the 5 MB limit.");
                }

                // Read image and convert to Base64
                using var memoryStream = new MemoryStream();
                image.CopyTo(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);

                // Compress image

                return Ok(base64String);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }
    }
}
