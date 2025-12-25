using System.ComponentModel.DataAnnotations;

namespace RightBrothersProduction.Models
{
    public class UserRegisterationModel
    {
        [Required]
        [MinLength(3)]
        public string FullName { get; set; }

        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number.")]
        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }



    }
}
