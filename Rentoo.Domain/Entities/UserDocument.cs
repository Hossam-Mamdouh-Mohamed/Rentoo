using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Entities
{
    public class UserDocument
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "License Document is required")]
        [Display(Name = "License Document")]
        public string LicenseUrl { get; set; }
        [Required(ErrorMessage = "National ID Number is required")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "National ID Number must be exactly 14 digits")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "National ID Number must contain exactly 14 digits")]
        public string NationalIDNumber { get; set; }
        [Required(ErrorMessage = "National ID Document is required")]
        [Display(Name = "National ID Document")]        public string NationalIDUrl { get; set; }

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
