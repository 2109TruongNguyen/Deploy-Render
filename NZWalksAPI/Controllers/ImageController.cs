using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Controllers;
    
[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IImageService _imageService;

    public ImageController(IImageService imageService)
    {
        _imageService = imageService;
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto uploadRequestDto)
    {
        ValidateFileUpload(uploadRequestDto);
        if (ModelState.IsValid)
        {
            // Convert to domain model
            var image = new Image()
            {
                File = uploadRequestDto.File,
                FileExtension = Path.GetExtension(uploadRequestDto.File.FileName),
                FileSizeInByte = uploadRequestDto.File.Length,
                FileName = uploadRequestDto.FileName,
                FileDescription = uploadRequestDto.FileDescription
            };

            await _imageService.UploadImage(image);

            return Ok(image);
        }
        
        return Ok(ModelState); 
    }

    private void ValidateFileUpload(ImageUploadRequestDto request)
    {
        var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        if (!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
        {
            ModelState.AddModelError("file", "Unsupported file extension");
        }

        if (request.File.Length > 10485760)
        {
            ModelState.AddModelError("file", "File size more than 10MB, please upload a smaller size file.");
        }
    }
}