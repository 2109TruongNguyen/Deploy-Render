using System.Text.Json.Serialization;

namespace NZWalksAPI.Models.Dto.Request
{
    public class AddRequestRegionDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? RegionImageUrl { get; set; }
    }
}
