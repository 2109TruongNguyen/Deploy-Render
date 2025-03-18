using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;

namespace NZWalksAPI.Services.Base
{
    public interface IRegionService
    {
        Task<List<Region>> GetAllAsync();
        Task<Region?> GetByIdAsync(Guid regionId);
        Task<Region> Update(Guid regionId, UpdateRegionRequestDto region);
        Task<Region> Create(Region region);
        Task DeleteAsync(Guid regionId);
    }
}
