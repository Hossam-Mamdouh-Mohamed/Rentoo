using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rentoo.Domain.Entities
{
    [Table("RateCodes")]
    public class RateCode
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public ICollection<RateCodeDay> RateCodeDays { get; set; }
    }
}
