using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Entities
{
    [Table("RateCodeDays")]
    public class RateCodeDay
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int RateCodeId { get; set; }
        [Required]
        [StringLength(20)]
        public int DayId { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; }

        [ForeignKey("RateCodeId")]
        public RateCode RateCode { get; set; }
    }
}
