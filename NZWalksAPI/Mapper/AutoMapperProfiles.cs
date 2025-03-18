using AutoMapper;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;
using NZWalksAPI.Models.Dto.Response;

namespace NZWalksAPI.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Region
            CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<Region, AddRequestRegionDto>().ReverseMap();
            CreateMap<Region, UpdateRegionRequestDto>().ReverseMap();
            
            // Walk
            CreateMap<Walk, AddWalkRequestDto>().ReverseMap();
            CreateMap<Walk, UpdateWalkRequestDto>().ReverseMap()
                .ForAllMembers(opts 
                    => opts.Condition((src, dest, srcMember) => srcMember != null));;
            CreateMap<Walk, WalkDto>().ReverseMap();
            
            // User
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
