using Application.DTOs.Image;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Drawing;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace HomeInvManagementAPI.Controllers
{
    [Route("[controller]/v1")]
    [ApiController]
    public class ImageController : Controller
    {
        // Injections 
        private readonly ILogger<ImageController> _logger;
        private readonly IImageCommand _imageCommand;
        private readonly IImageQuery _imageQuery;

        public ImageController(ILogger<ImageController> logger, IImageCommand imageCommand, IImageQuery imageQuery)
        {
            _logger = logger;
            _imageCommand = imageCommand;
            _imageQuery = imageQuery;
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadImageToDBAsync(IFormFile image)
        {
            try
            {
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

                var inputDto = new ImageUploadInDTO
                {
                    ImageBytes = imageBytes,
                    Extension = imageExt
                };

                var outDTO = await _imageCommand.UploadImageAsync(inputDto);

                return StatusCode(StatusCodes.Status200OK, outDTO);
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("download/{imageId}")]
        public async Task<ActionResult> DownloadImageFromDBAsync([FromRoute]int imageId)
        {
            try
            {
                if (imageId <= 0)
                    return BadRequest("Invalid image ID.");
                
                var inDTO = new ImageDownloadInDTO
                {
                    ImageId = imageId
                };

                var outDTO = await _imageQuery.DownloadImageAsync(inDTO);

                if (outDTO?.ImageBytes == null || outDTO.ImageBytes.Length == 0)
                    return NotFound("Image not found.");

                var fileName = $"image_{imageId}{outDTO.Extension}";

                var selectedContentType = outDTO.Extension.ToLowerInvariant() switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                return File(
                    fileContents: outDTO.ImageBytes,
                    contentType: selectedContentType,
                    fileDownloadName: fileName
                );
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{imageId}")]
        public async Task<ActionResult> DeleteImageFromDBAsync([FromRoute] int imageId)
        {
            try
            {
                if (imageId <= 0)
                    return BadRequest("Invalid image ID.");

                var result = await _imageCommand.DeleteImageAsync(imageId);
                if (!result)
                    return NotFound("Image not found or could not be deleted.");
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Internal server error: {ex.Message}");
            }
        }
}
