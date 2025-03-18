using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Services.Base;

public interface IImageService 
{
    Task<Image> UploadImage(Image image);
}