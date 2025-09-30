using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.Models
{
    public class NormalRequestVote
    {
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Required]
        public int RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public NormalRequest Request { get; set; }

        [Required]
        public DateTime VotedAt { get; set; }
    }
}
