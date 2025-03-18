using NZWalksAPI.Models.Domain;

namespace NZWalksAPI.Repositories.Base
{
    public interface IWalkRepository
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
        Task<Walk?> UpdateAsync(Walk? walk);
        Task<Walk?> CreateAsync(Walk? walk);
        Task<bool> DeleteAsync(Guid walkId);
    } 
}
