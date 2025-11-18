using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.API.DTOs
{
    public class RequestFileDto
    {
        public int Id { get; set; }
        public string FileUrl { get; set; }
        public string FileName { get; set; }
    }
}
