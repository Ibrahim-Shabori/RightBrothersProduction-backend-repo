using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RightBrothersProduction.Models.RequestModels;

namespace RightBrothersProduction.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [MaxLength(7)]
        public string Color { get; set; } = "#3B82F6";

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }

        public RequestType requestType { get; set; }

        public ICollection<Request> Requests { get; set; } = new List<Request>();


    }
}
