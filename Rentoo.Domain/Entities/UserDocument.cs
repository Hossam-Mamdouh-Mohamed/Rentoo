using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Entities
{
    public class UserDocument
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(100)]
        public string LicenseUrl { get; set; }
        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numeric characters are allowed.")]
        [MaxLength(100)]
        public string NationalIDNumber { get; set; }
        [Required]
        [StringLength(200)]
        public string NationalIDUrl { get; set; }

        [MaxLength(200)]
        public string? Notes { get; set; }
        public UserDocumentStatus Status { get; set; } = UserDocumentStatus.Pending;
        public DateTime? ReviewdAt { get; set; }
        [MaxLength(300)]
        public string? ReviewNotes { get; set; }
        public string? UserId { get; set; }
        [ForeignKey("UserId")] 
        public User? User { get; set; }
    }
    public enum UserDocumentStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
