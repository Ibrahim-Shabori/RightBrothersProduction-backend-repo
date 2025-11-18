using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightBrothersProduction.Models
{
    public class RequestLog
    {
        [Key]
        public int Id { get; set; }

        public int RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public Request Request { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Comment { get; set; }
    }
}
