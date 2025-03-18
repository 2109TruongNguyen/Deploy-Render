using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;

namespace NZWalksAPI.Repositories.Base
{
    public interface IRegionRepository
    {
        Task<List<Region>> GetAllAsync();
        Task<Region?> GetByIdAsync(Guid regionId);
        Task<Region> Update(Guid regionId, UpdateRegionRequestDto region);
        Task<Region> Create(Region region);
        Task DeleteAsync(Guid regionId);
    }
}
