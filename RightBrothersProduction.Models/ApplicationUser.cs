
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace RightBrothersProduction.Models
{
    public class ApplicationUser:IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string? FullName { get; set; }

        public ICollection<Request> RequestsCreated { get; set; } = new List<Request>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<RegisteredRequest> RegisteredRequests { get; set; } = new List<RegisteredRequest>();
    }
}
