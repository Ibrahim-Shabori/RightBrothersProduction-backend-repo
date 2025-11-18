

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.Models
{
    public class DetailedRequest
    {
        [Key]
        [ForeignKey(nameof(Request))] // 1-to-1 relationship
        public int RequestId { get; set; }

        [Required]                 // ensure it’s not null in DB
        [MaxLength(4000)]          // pick a reasonable size for long text
        public string DetailedDescription { get; set; }

        public int UsageDurationInMonths { get; set; }

        [MaxLength(500)]
        public string UrgencyCause { get; set; }

        [MaxLength(2000)]
        public string AdditionalNotes { get; set; }

        [Phone]                    // data annotation for validation (not enforced in DB)
        [MaxLength(30)]
        public string ContributerPhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string ContributerEmail { get; set; }

        public Request Request { get; set; }
    }
}
