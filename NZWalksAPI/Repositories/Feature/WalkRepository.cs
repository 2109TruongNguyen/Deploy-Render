using Microsoft.EntityFrameworkCore;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Repositories.Base;

namespace NZWalksAPI.Repositories.Feature
{
    public class WalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;

        public WalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<Walk?>> GetAllAsync(
            string? filterOn = null, 
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = false,
            int pageNumber = 1,
            int pageSize = 10
            )
        {
            var walks = dbContext.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .AsQueryable();
            // Filter
            if(!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks= walks.Where(w => w.Name.Contains(filterQuery));
                }
            }
            
            // Sort
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }
            
            var skipRecords = (pageSize - 1) * pageSize;
            
            return await walks.Skip(skipRecords).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(Guid walkId) 
            => await dbContext.Walks
                .Include(w => w.Difficulty)
                .Include(w => w.Region)
                .FirstOrDefaultAsync( w => w.Id.Equals(walkId));  

        public async Task<Walk?> UpdateAsync(Walk? walk)
        {
            dbContext.Walks.Update(walk);
            await dbContext.SaveChangesAsync();

            return walk;
        }
        
        public async Task<Walk?> CreateAsync(Walk? walk)
        {
            await dbContext.Walks.AddAsync(walk);
            await dbContext.SaveChangesAsync();

            return walk;
        }

        public async Task<bool> DeleteAsync(Guid walkId)
        {
            var walk = await GetByIdAsync(walkId);

            if (walk != null)
            {
                dbContext.Walks.Remove(walk);
                await dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
