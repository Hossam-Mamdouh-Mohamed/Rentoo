using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Models
{
    [Table("CarDocuments")]
    public class CarDocument
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(500)]
        public string LicenseUrl { get; set; }

        [Required]
        public int LicenseNumber { get; set; }

        [Required]
        public DocumentStatus status { get; set; } = DocumentStatus.Pending;

        public DateTime? ReviewdAt { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public int CarId { get; set; }
        public Car Car { get; set; }
        public int? UserId { get; set; }
        public User? User { get; set; }

    }

    public enum DocumentStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
