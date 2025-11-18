using System;
using System.Collections.Generic;
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

        public string Description { get; set; }

        public RequestType requestType { get; set; }

        public ICollection<Request> Requests { get; set; } = new List<Request>();


    }
}
