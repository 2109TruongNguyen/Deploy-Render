using System.Text.Json;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalksAPI.Models.Domain;
using NZWalksAPI.Models.Dto.Request;
using NZWalksAPI.Repositories.Base;
using NZWalksAPI.Services;
using NZWalksAPI.Services.Base;

namespace NZWalksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class RegionsController : ControllerBase
    {
        private readonly IRegionService _regionService;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IOrderService _orderService;

        public RegionsController(IRegionService regionService, IMapper mapper, ILogger<RegionsController> logger, 
            IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager, IOrderService orderService)
        {
            this._regionService = regionService;
            this.mapper = mapper;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _orderService = orderService;
        }

        // GET All REGIONS
        // GET: https://localhost:portnumber/api/regions
        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAllRegions()
        {   
            // Get Data from Database
            _recurringJobManager.AddOrUpdate(
                "process_orders",
                () => Console.WriteLine($"🕒 Processing orders at {DateTime.Now}"),
                Cron.MinuteInterval(1)); // Run every 1 minute
            
            var regions = await _regionService.GetAllAsync();
            _logger.LogInformation($"fINISHED Getting all regions WITH DATA: {JsonSerializer.Serialize(regions)}");
            return Ok(mapper.Map<List<RegionDto>>(regions));
        }

        // GET REGION BY ID
        // GET: https://localhost:portnumber/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetRegionById([FromRoute] Guid id)
        {
            //var region = await _dbContext.Regions.FindAsync(id);

            var region = await _regionService.GetByIdAsync(id);

            if (region == null)
            {
                return NotFound();
            }

            //Return DTO
            return Ok(mapper.Map<RegionDto>(region));
        }

        // POST TO CREATE NEW REGION
        // GET: https://localhost:portnumber/api/regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRequestRegionDto addRequest)
        {
            //Map or convert DTO to Domain Model
            var region = mapper.Map<Region>(addRequest);

            // Using service to save new Region to Database
            region = await _regionService.Create(region);

            //Map Domain Model to DTO
            var regionDto = mapper.Map<RegionDto>(region);  

            return CreatedAtAction(nameof(GetRegionById), new { id = regionDto.Id }, regionDto);
        }

        // PUT to UPDATE REGION
        // PUT: https://localhost:portnumber/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto addRequest)
        {
            // Find Region
            var region = await _regionService.GetByIdAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            // Use service to update Region
            region = await _regionService.Update(id, addRequest);
            
            // COnvert Domain Model to DTO
            
            return Ok(mapper.Map<RegionDto>(region));
        }

        // DELETE REGION
        // DELETE: https://localhost:portnumber/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            //Find Region
            var region = await _regionService.GetByIdAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            //Remove Region
            await _regionService.DeleteAsync(id);

            // Return deleted region
            
            return Ok(mapper.Map<RegionDto>(region));
        }
    }
}
