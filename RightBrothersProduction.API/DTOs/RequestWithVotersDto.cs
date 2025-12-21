using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class RequestWithVotersDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int VotesCount { get; set; }
        public RequestType Type { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public string CreatedById { get; set; }

        public List<RequestFileDto> Files { get; set; }
        public List<string> Voters { get; set; } = new List<string>();

    }
}
