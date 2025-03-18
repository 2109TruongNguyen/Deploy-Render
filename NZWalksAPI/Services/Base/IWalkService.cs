using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;

namespace NZWalksAPI.Services.Base
{
    public interface IWalkService
    {
        Task<List<Walk?>> GetAllAsync(
            string? filterOn, 
            string? filterQuery,
            string? sortBy,
            bool isAscending,
            int pageNumber,
            int pageSize
        );
        Task<Walk?> GetByIdAsync(Guid walkId);
        Task<Walk?> UpdateAsync(Guid id, UpdateWalkRequestDto walk);
        Task<Walk?> CreateAsync(AddWalkRequestDto walk);
        Task<bool> DeleteAsync(Guid walkId);
    }
}
