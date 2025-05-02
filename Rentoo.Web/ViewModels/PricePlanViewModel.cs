using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Rentoo.Web.ViewModels
{
    public class PricePlanViewModel : IValidatableObject
    {
        public int ID { get; set; }
        
        [Required(ErrorMessage = "Plan name is required")]
        [StringLength(50, ErrorMessage = "Plan name cannot exceed 50 characters")]
        public string Name { get; set; }

        public List<PricePlanDayViewModel> Days { get; set; } = new List<PricePlanDayViewModel>
        {
            new PricePlanDayViewModel { DayId = 1, DayName = "Saturday", Price = 0 },
            new PricePlanDayViewModel { DayId = 2, DayName = "Sunday", Price = 0 },
            new PricePlanDayViewModel { DayId = 3, DayName = "Monday", Price = 0 },
            new PricePlanDayViewModel { DayId = 4, DayName = "Tuesday", Price = 0 },
            new PricePlanDayViewModel { DayId = 5, DayName = "Wednesday", Price = 0 },
            new PricePlanDayViewModel { DayId = 6, DayName = "Thursday", Price = 0 },
            new PricePlanDayViewModel { DayId = 7, DayName = "Friday", Price = 0 }
        };

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Check if any day has a price of 0
            var daysWithZeroPrice = Days.Where(d => d.Price == 0).ToList();
            if (daysWithZeroPrice.Any())
            {
                foreach (var day in daysWithZeroPrice)
                {
                    results.Add(new ValidationResult(
                        $"Please enter a price for {day.DayName}",
                        new[] { $"Days[{Days.IndexOf(day)}].Price" }
                    ));
                }
            }

            return results;
        }
    }

    public class PricePlanDayViewModel
    {
        public int ID { get; set; }
        public int DayId { get; set; }
        public string DayName { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public float Price { get; set; }
    }
} 