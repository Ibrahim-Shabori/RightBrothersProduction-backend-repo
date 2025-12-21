using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.API.DTOs
{
    public class UpdateRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public List<int> ExistingAttachmentIds { get; set; } = new();
        public List<IFormFile> NewAttachments { get; set; } = new();
    }
}
