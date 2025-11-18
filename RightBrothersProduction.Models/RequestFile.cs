
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RightBrothersProduction.Models
{
    public class RequestFile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]           // Avoid unbounded nvarchar(max)
        public string FileName { get; set; }      // e.g., "a34f.jpg"

        [Required]
        [MaxLength(100)]           // "image/jpeg", "application/pdf", etc.
        public string ContentType { get; set; }

        [Range(1, 5 * 1024 * 1024)]   // 1 byte .. 10 MB (change if needed)
        public long Size { get; set; }

        public int RequestId { get; set; }

        [ForeignKey(nameof(RequestId))]
        public Request Request { get; set; }
    }
}
