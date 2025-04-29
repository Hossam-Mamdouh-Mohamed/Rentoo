using System.ComponentModel.DataAnnotations;

namespace Rentoo.Web.ViewModels
{
    public class PricePlanViewModel
    {
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public List<PricePlanDayViewModel> Days { get; set; }
    }

    public class PricePlanDayViewModel
    {
        public int ID { get; set; }
        [Required]
        public int DayId { get; set; }
        [Required]
        [Range(0, float.MaxValue)]
        public float Price { get; set; }
    }
} 