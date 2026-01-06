using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class CreateRequestLogDto
    {
        public int RequestId { get; set; }
        public RequestStatus NewStatus { get; set; }
        public string? Comment { get; set; }
        public bool IsPublic { get; set; }
    }
}
