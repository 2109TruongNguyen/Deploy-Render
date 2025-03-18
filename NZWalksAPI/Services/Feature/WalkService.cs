using AutoMapper;
using NZWalksAPI.Mapper;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;
using NZWalksAPI.Repositories.Base;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Services.Feature
{
    public class WalkService : IWalkService
    {
        private readonly IWalkRepository _walkRepository;
        private readonly IMapper _mapper;

        public WalkService(IWalkRepository walkRepository, IMapper mapper)
        {
            this._walkRepository = walkRepository;
            this._mapper = mapper;
        }

        public async Task<List<Walk?>> GetAllAsync(
            string? filterOn = null, 
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = false,
            int pageNumber = 1,
            int pageSize = 10
            ) 
            => await _walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);

        public async Task<Walk?> GetByIdAsync(Guid walkId) => await _walkRepository.GetByIdAsync(walkId);

        public async Task<Walk?> UpdateAsync(Guid id, UpdateWalkRequestDto walk)
        {
            var walkEntity = await _walkRepository.GetByIdAsync(id);
            
            if (walkEntity == null)
            {
                return null;
            }
            var updateWalkEntity = _mapper.Map(walk, walkEntity);
            return await _walkRepository.UpdateAsync(updateWalkEntity);
        }
        
        
        
        public async Task<Walk?> CreateAsync(AddWalkRequestDto walkAddWalkRequest) 
            => await _walkRepository.CreateAsync(_mapper.Map<Walk>(walkAddWalkRequest));

        public async Task<bool> DeleteAsync(Guid walkId) => await _walkRepository.DeleteAsync(walkId);
    }
}
