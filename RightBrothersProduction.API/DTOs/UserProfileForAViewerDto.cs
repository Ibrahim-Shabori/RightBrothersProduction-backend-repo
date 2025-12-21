namespace RightBrothersProduction.API.DTOs
{
    public class UserProfileForAViewerDto
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public int TotalRequests { get; set; }
        public int TotalVotesReceived { get; set; }
        public int ImplementedRequests { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateJoined { get; set; }
    }
}
