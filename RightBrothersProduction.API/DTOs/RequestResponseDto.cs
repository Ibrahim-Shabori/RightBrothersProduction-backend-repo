using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class RequestResponseDto
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

        public bool IsVotedByCurrentUser { get; set; } = false;

        public List<RequestFileDto> Files { get; set; }
    }
}
