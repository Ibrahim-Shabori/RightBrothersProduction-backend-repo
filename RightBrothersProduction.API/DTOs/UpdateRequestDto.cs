using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.API.DTOs
{
    public class UpdateRequestDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }

        public int CategoryId { get; set; }

        // Type = regular request or detailed request

        // Files uploaded from the frontend
        public List<IFormFile>? Attachments { get; set; }
        public string? DetailedDescription { get; set; }
        public int? UsageDurationInMonths { get; set; }
        public string? UrgencyCause { get; set; }
        public string? AdditionalNotes { get; set; }
        public string? ContributerPhoneNumber { get; set; }
        public string? ContributerEmail { get; set; }
        public List<int>? OldFilesToDelete { get; set; }
    }
}
