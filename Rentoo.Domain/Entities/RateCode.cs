using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Models
{
    [Table("RateCodes")]
    public class RateCode
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<RateCodeDay> RateCodeDays { get; set; }
    }
}
