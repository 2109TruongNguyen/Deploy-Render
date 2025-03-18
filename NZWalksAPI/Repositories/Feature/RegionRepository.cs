using Microsoft.EntityFrameworkCore;
using NZWalksAPI.Data;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;
using NZWalksAPI.Repositories.Base;

namespace NZWalksAPI.Repositories.Feature
{
    public class RegionRepository : IRegionRepository
    {
        private readonly NZWalksDbContext dbContext;
        public RegionRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // CRUD
        public async Task<Region> Create(Region region)
        {
            //Use Domain Model to save to Database
            await dbContext.Regions.AddAsync(region);
            await dbContext.SaveChangesAsync();

            return region;
        }

        public async Task DeleteAsync(Guid regionId)
        {
            var region = await GetByIdAsync(regionId);
            dbContext.Regions.Remove(region);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Region>> GetAllAsync()
        {
            return await dbContext.Regions.ToListAsync();
        }

        public async Task<Region?> GetByIdAsync(Guid regionId)
        { 
            return await dbContext.Regions.FirstOrDefaultAsync(_ => _.Id.Equals(regionId));
        }

        public async Task<Region> Update(Guid regionId, UpdateRegionRequestDto region)
        {
            var regionUpdate = await GetByIdAsync(regionId);

            if (regionUpdate != null)
            {
                //Update Region
                regionUpdate.Code = region.Code;
                regionUpdate.Name = region.Name;
                regionUpdate.RegionImageUrl = region.RegionImageUrl;
                
                //Save Changes
                await dbContext.SaveChangesAsync();
            }
            return regionUpdate;
        }
    }
}
