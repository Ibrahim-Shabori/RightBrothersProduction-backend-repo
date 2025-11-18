using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.Models
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [Required]
        public RequestStatus Status { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int VotesCount { get; set; }

        // 1-to-1 relationship
        public DetailedRequest DetailedRequest { get; set; }

        // Assignment relationship
        public RegisteredRequest RegisteredRequest { get; set; }

        // Type discriminator
        public RequestType Type { get; set; }

        // ---------- Category ----------
        [Required]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }


        // ---------- CreatedBy ----------
        [Required]
        public string CreatedById { get; set; }

        [ForeignKey(nameof(CreatedById))]
        public ApplicationUser CreatedBy { get; set; }
        // -------------------------------

        public ICollection<RequestFile> Files { get; set; } = new List<RequestFile>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<RequestLog> Logs { get; set; } = new List<RequestLog>();
    }
}
