using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class RequestLogDetailsDto
    {
        public string? Comment { get; set; }
        public RequestStatus NewStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; }
        public string LoggerName { get; set; }
        public string? LoggerPictureUrl { get; set; }
    }
}
