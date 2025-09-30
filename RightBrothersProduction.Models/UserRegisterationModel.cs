using System.ComponentModel.DataAnnotations;

namespace RightBrothersProduction.Models
{
    public class UserRegisterationModel
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }



    }
}
