using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Repositories.Base;

public interface IImageRepository
{
    Task<Image> UploadImage(Image image);
}