using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Models
{
    [Table("CarImages")]
    public class CarImage
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; }

        [ForeignKey("CarId")]
        public Car Car { get; set; }
    }
}
