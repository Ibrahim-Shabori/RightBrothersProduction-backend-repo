using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.Models
{
    public class RegisteredRequest
    {
        [Key]
        public int RequestId { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public Request Request { get; set; }

        public string AssignedToId { get; set; }
        [ForeignKey(nameof(AssignedToId))]
        public ApplicationUser AssignedTo { get; set; }
    }
}
