

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

        [Required]                
        [MaxLength(4000)]          
        public string DetailedDescription { get; set; }

        public int UsageDurationInMonths { get; set; }

        [MaxLength(500)]
        public string UrgencyCause { get; set; }

        [MaxLength(2000)]
        public string AdditionalNotes { get; set; }
                  
        [MaxLength(30)]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number.")]
        public string ContributerPhoneNumber { get; set; }

        [EmailAddress]
        [MaxLength(255)]
        public string ContributerEmail { get; set; }

        public Request Request { get; set; }
    }
}
