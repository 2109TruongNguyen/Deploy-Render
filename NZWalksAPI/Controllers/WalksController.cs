using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;
using NZWalksAPI.Models.Dto.Response;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkService _walkService;
        
        public WalksController(IMapper mapper, IWalkService walkService)
        {
            this.mapper = mapper;
            _walkService = walkService;
        }
        
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllWalks(
            [FromQuery] string? filterOn,
            [FromQuery] string? filterQuery,
            [FromQuery] string? sortBy,
            [FromQuery] bool isAscending,
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize
            )
        {
            try
            {
                var walks = await _walkService.GetAllAsync(
                    filterOn, filterQuery, sortBy, isAscending, pageNumber, pageSize);
            
                return Ok(ApiResponse<List<WalkDto>>.Success(
                    HttpStatusCode.OK,
                    "Successfully retrieved all walks",
                    mapper.Map<List<WalkDto>>(walks)
                ));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        [HttpGet("get-by-id/{id:Guid}")]
        public async Task<IActionResult> GetWalkById([FromRoute] Guid id)
        {
            var walk = await _walkService.GetByIdAsync(id);
            if (walk == null)
            {
                return NotFound();
            }
            return Ok(ApiResponse<WalkDto>.Success(
                HttpStatusCode.OK,
                "Successfully retrieved all walks",
                mapper.Map<WalkDto>(walk)
            ));
        }
        
        [HttpPost("create-walk")]
        public async Task<IActionResult> CreateWalk([FromBody] AddWalkRequestDto walk)
        {
            var newWalk = await _walkService.CreateAsync(walk);
            var res = ApiResponse<WalkDto>.Success(
                HttpStatusCode.Created,
                "Successfully created walk",
                mapper.Map<WalkDto>(newWalk)
            );
            
            return Ok(res);
        }
        
        [HttpPut("update-walk/{id:Guid}")]
        public async Task<IActionResult> UpdateWalk([FromRoute] Guid id, [FromBody] UpdateWalkRequestDto walk)
        {
            var updatedWalk = await _walkService.UpdateAsync(id, walk);
            
            //return updatedWalk != null ? Ok(mapper.Map<WalkDto>(updatedWalk)): Ok("Walk not found");
            return Ok(ApiResponse<Object>.Success(
                updatedWalk != null ? HttpStatusCode.OK : HttpStatusCode.NotFound,
                updatedWalk != null ? "Walk updated" : "Walk not found",
                mapper.Map<WalkDto>(updatedWalk)
            ));
        }
        
        [HttpDelete("delete-walk/{id:Guid}")]
        public async Task<IActionResult> DeleteWalk([FromRoute] Guid id)
        {
            var deleted = await _walkService.DeleteAsync(id);
            //return deleted ? Ok("Walk deleted") : Ok("Walk not found");
            return Ok(ApiResponse<Object>.Success(
                deleted ? HttpStatusCode.OK : HttpStatusCode.NotFound,
                deleted ? "Walk deleted" : "Walk not found",
                null
            ));
        }
    }
}
