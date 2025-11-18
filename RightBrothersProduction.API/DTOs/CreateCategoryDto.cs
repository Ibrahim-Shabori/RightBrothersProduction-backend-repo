using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public RequestType Type { get; set; }
    }
}
