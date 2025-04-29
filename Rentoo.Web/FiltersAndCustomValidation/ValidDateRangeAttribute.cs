using Rentoo.Web.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Rentoo.Web.FiltersAndCustomValidation
{
  
    public class ValidDateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = (RequestViewModel)validationContext.ObjectInstance;

            // Check if start date is in the future
            if (model.StartDate < DateTime.Today)
            {
                return new ValidationResult("Start date must be today or a future date.");
            }

            // Check if end date is in the future
            if (model.EndDate < DateTime.Today)
            {
                return new ValidationResult("End date must be today or a future date.");
            }

            // Check if start date is before end date
            if (model.StartDate > model.EndDate)
            {
                return new ValidationResult("Start date must be before or equal to end date.");
            }

            return ValidationResult.Success;
        }
    }

}
