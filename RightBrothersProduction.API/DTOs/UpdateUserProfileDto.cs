using System.ComponentModel.DataAnnotations;

namespace RightBrothersProduction.API.DTOs
{
    public class UpdateUserProfileDto
    {
        [Required]
        [MinLength(3)]
        public string FullName { get; set; }

        // Optional: Only include if the user wants to change it
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }

        // Optional: Only needed if they are trying to set a NewPassword
        public string? CurrentPassword { get; set; }

        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string? NewPassword { get; set; }
    }
}
