

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.Models
{
    public class DetailedRequest : IRequest
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


        [Required]                 // ensure it’s not null in DB
        [MaxLength(4000)]          // pick a reasonable size for long text
        public string DetailedDescription { get; set; }

        [Range(0, 120)]            // months of use, e.g., 0–10 years
        public int UseTimeInMonths { get; set; }

        [MaxLength(500)]
        public string? UrgencyCause { get; set; }

        [MaxLength(2000)]
        public string? AdditionalNotes { get; set; }

        [Phone]                    // data annotation for validation (not enforced in DB)
        [MaxLength(30)]
        public string? ContributerPhoneNumber { get; set; }

        [EmailAddress]             // for MVC validation
        [MaxLength(255)]
        public string? ContributerEmail { get; set; }
    }
}
