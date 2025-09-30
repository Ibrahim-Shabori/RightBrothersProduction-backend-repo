
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.Models
{
    public class NormalRequest:IRequest
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
        public RequestType Type { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // ---------- Navigation: Files ----------
        public ICollection<RequestFile> Files { get; set; }

        public int VotesCount { get; set; }


        // ---------- CreatedBy ----------
        [ForeignKey(nameof(CreatedBy))]
        public string? CreatedById { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        // ---------- RegisteredAdmin ----------
        [ForeignKey(nameof(RegisteredAdmin))]
        public string? RegisteredAdminId { get; set; }
        public ApplicationUser? RegisteredAdmin { get; set; }
    }
}
