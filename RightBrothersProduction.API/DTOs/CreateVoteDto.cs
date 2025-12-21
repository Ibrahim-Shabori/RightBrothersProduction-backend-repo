namespace RightBrothersProduction.API.DTOs
{
    public class CreateVoteDto
    {
        public int RequestId { get; set; }
        public string UserId { get; set; }
        public DateTime VotedAt { get; set; }
    }
}
