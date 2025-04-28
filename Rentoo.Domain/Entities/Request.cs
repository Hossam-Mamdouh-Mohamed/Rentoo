using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Entities
{
    [Table("Requests")]
    public class Request
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string StartDate { get; set; }

        [Required]
        public string EndDate { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float TotalPrice { get; set; }

        [Required]
        [StringLength(200)]
        public string DeliveryAddress { get; set; }

        [Required]
        [StringLength(200)]
        public string pickupAddress { get; set; }

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [Required]
        public bool WithDriver { get; set; }

        [Required]
        public string? UserID { get; set; }

        [ForeignKey("UserID")]
        public User ?User { get; set; }


        public int? CarId { get; set; }

        [ForeignKey("CarId")]
        public Car ?Car { get; set; }

        public int? reviewId { get; set; }
        
        [ForeignKey("reviewId")]
        public RequestReview? Review { get; set; }
    }

    public enum RequestStatus
    {
        Pending,
        Accepted,
        Rejected,
        Completed
    }
}
