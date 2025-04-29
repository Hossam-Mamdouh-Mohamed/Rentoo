using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Entities
{
    [Table("RequestReviews")]
    public class RequestReview
    {
        [Key]
        public int ID { get; set; }


        public int ?RequestId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        [ForeignKey("RequestId")]
        public Request? Request { get; set; }
    }
}
