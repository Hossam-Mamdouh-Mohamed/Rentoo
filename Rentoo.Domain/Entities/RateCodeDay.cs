using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Models
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
        public string Day { get; set; }

        [ForeignKey("RateCodeId")]
        public RateCode RateCode { get; set; }
    }
}
