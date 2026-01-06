using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class ReviewRequestDto
    {
        public RequestStatus NewStatus { get; set; }
        public string? Comment { get; set; }
    }
}
