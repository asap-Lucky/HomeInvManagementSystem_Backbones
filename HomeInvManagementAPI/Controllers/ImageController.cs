using Application.DTOs.Image;
using Application.DTOs.Outbound;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IConfiguration _configuration;

        // Configuration keys
        private readonly List<string> _permImageUploadExt;
        private readonly long _maxImageUploadSize;

        public ImageController(ILogger<ImageController> logger, IImageCommand imageCommand, IImageQuery imageQuery, IConfiguration configuration)
        {
            _logger = logger;
            _imageCommand = imageCommand;
            _imageQuery = imageQuery;
            _configuration = configuration;

            _permImageUploadExt = _configuration.GetSection("ImageController:PermittedUploadExt").Get<List<string>>() ?? throw new InvalidOperationException("ImageController: Permitted extensions for image upload are missing.");
            _maxImageUploadSize = _configuration.GetValue<long?>("ImageController:MaxImgSizeBytes") ?? throw new InvalidOperationException("ImageController: Max image upload size not set");
        }

        [HttpPost("upload")]
        [ProducesResponseType<ImageUploadOutDTO>(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ImageUploadOutDTO>> UploadImageToDBAsync(IFormFile image)
        {
            try
            {
                // Supported extensions and size limit
                List<string> permittedExtensions = _permImageUploadExt;
                long fileSizeLimit = _maxImageUploadSize;

                string? imageExt = Path.GetExtension(image.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(imageExt) || !permittedExtensions.Contains(imageExt))
                    return StatusCode(StatusCodes.Status415UnsupportedMediaType);

                if (image.Length > fileSizeLimit)
                    return StatusCode(StatusCodes.Status413PayloadTooLarge);

                // Read image and convert to Base64
                using var memoryStream = new MemoryStream();
                image.CopyTo(memoryStream);
                byte[] imageBytes = memoryStream.ToArray();

                var inputDto = new ImageUploadInDTO
                {
                    ImageBytes = imageBytes,
                    Extension = imageExt
                };

                ImageUploadOutDTO outDTO = await _imageCommand.UploadImageAsync(inputDto);

                return StatusCode(StatusCodes.Status201Created, outDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        [HttpGet("download/{imageId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DownloadImageFromDBAsync([FromRoute] int imageId)
        {
            try
            {
                if (imageId <= 0)
                    return StatusCode(StatusCodes.Status400BadRequest, "ImageId must be above 0");

                var inDTO = new ImageDownloadInDTO
                {
                    ImageId = imageId
                };

                ImageDownloadOutDTO outDTO = await _imageQuery.DownloadImageAsync(inDTO);

                if (outDTO?.ImageBytes == null || outDTO.ImageBytes.Length == 0)
                    return StatusCode(StatusCodes.Status404NotFound, $"ImageId with id {imageId} was not found");

                // Set appropriate content type based on the image extension & file name
                string? fileName = $"image_{imageId}{outDTO.Extension}";

                // Create a mapping of extensions to content types.
                string? selectedContentType = outDTO.Extension.ToLowerInvariant() switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                FileContentResult fileResult = File(fileContents: outDTO.ImageBytes, contentType: selectedContentType,fileDownloadName: fileName);

                return StatusCode(StatusCodes.Status200OK, fileResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{imageId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteImageFromDBAsync([FromRoute] int imageId)
        {
            try
            {
                if (imageId <= 0)
                    return StatusCode(StatusCodes.Status400BadRequest, "ImageId must be above 0");

                var inDTO = new ImageDeleteInDTO
                {
                    ImageId = imageId
                };

                ImageDeleteOutDTO? outDTO = await _imageCommand.DeleteImageAsync(inDTO);

                if (outDTO == null)
                    return StatusCode(StatusCodes.Status404NotFound, $"Image with id {imageId} was not found and therefore not deleted.");

                if (!outDTO.IsDeleted)
                    return StatusCode(StatusCodes.Status409Conflict, $"Image could not be deleted on server. It may still be in use by other objects.");

                return StatusCode(StatusCodes.Status204NoContent, outDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Internal server error: {ex.Message}");
            }
        }
    }
}