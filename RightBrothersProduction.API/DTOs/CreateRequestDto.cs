using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class CreateRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public int CategoryId { get; set; }

        // Type = regular request or detailed request
        public RequestType Type { get; set; }

        // Files uploaded from the frontend
        public List<IFormFile>? Attachments { get; set; }
    }
}
