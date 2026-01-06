using System.ComponentModel.DataAnnotations;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Invalid Hex Color")]
        public string Color { get; set; }

        public RequestType Type { get; set; }
    }
}
