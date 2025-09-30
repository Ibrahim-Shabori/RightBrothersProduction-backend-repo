
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RightBrothersProduction.Models
{
    public class RequestFile
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]           // Avoid unbounded nvarchar(max)
        public string FileName { get; set; }      // e.g., "a34f.jpg"

        [Required]
        [MaxLength(100)]           // "image/jpeg", "application/pdf", etc.
        public string ContentType { get; set; }

        [Range(1, 5 * 1024 * 1024)]   // 1 byte .. 10 MB (change if needed)
        public long Size { get; set; }

        [ForeignKey(nameof(NormalRequest))]
        public int? NormalRequestId { get; set; }
        public NormalRequest? NormalRequest { get; set; }

        [ForeignKey(nameof(DetailedRequest))]
        public int? DetailedRequestId { get; set; }
        public DetailedRequest? DetailedRequest { get; set; }
    }
}
