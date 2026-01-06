using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class RequestPageItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public RequestType Type { get; set; }
        public bool IsDetailed { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByProfilePicture { get; set; }
    }

    public class RequestManagementPageItemDto { 
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public RequestType Type { get; set; }
        public RequestStatus Status { get; set; }
        public bool IsDetailed { get; set; }
        public int VotesCount { get; set; }
        public int TrendsCount { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public List<RequestLogForAdminsDto> Logs { get; set; }
    }

    public class UserManagementPageItemDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime? JoinedAt { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
        public int PerformanceScore { get; set; }
        public DateTime? LastActivity { get; set; }

    }
}
