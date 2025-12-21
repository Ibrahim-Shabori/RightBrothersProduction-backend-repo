namespace RightBrothersProduction.API.DTOs
{
    public class UserProfileDto
    {
        public string FullName { get; set; }
        public int TotalRequests { get; set; }
        public int TotalVotesReceived { get; set; }
        public int ImplementedRequests { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateJoined { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
