
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace RightBrothersProduction.Models
{
    public class ApplicationUser:IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        [Required]
        [MinLength(3)]
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateJoined { get; set; } = DateTime.UtcNow;
        public string? Bio { get; set; }

        public ICollection<Request> RequestsCreated { get; set; } = new List<Request>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<RegisteredRequest> RegisteredRequests { get; set; } = new List<RegisteredRequest>();
    }
}
