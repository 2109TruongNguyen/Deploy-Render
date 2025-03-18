using NZWalksAPI.Models.Domain;
using NZWalksAPI.Repositories.Base;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Services.Feature;

public class ImageService : IImageService
{
    private readonly IImageRepository _imageRepository;

    public ImageService(IImageRepository imageRepository)
    {
        _imageRepository = imageRepository;
    }
    
    
    public async Task<Image> UploadImage(Image image)
    {
        return await _imageRepository.UploadImage(image);
    }
}