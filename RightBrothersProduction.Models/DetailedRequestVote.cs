
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RightBrothersProduction.Models
{
    public class DetailedRequestVote
    {
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Required]
        public int RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public DetailedRequest Request { get; set; }

        [Required]
        public DateTime VotedAt { get; set; }
    }
}
