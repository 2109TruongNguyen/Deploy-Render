using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;
using NZWalksAPI.Repositories.Base;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Services.Feature
{
    public class RegionService : IRegionService
    {
        private readonly IRegionRepository _regionRepository;
        public RegionService(IRegionRepository _regionRepository) 
        { 
            this._regionRepository = _regionRepository;
        }
        
        // CRUD
        public async Task<Region> Create(Region region) => await _regionRepository.Create(region);

        public async Task DeleteAsync(Guid regionId) => await _regionRepository.DeleteAsync(regionId);

        public async Task<List<Region>> GetAllAsync() => await _regionRepository.GetAllAsync();

        public async Task<Region?> GetByIdAsync(Guid regionId) => await _regionRepository.GetByIdAsync(regionId);

        public async Task<Region> Update(Guid regionId, UpdateRegionRequestDto region) => await _regionRepository.Update(regionId, region);
    }
}
