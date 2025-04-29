using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Rentoo.Web.FiltersAndCustomValidation
{
    public class FileInputValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If the value is null, it's valid (optional in edit mode)
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // For IFormFile
            if (value is IFormFile file)
            {
                if (file.Length == 0)
                {
                    return ValidationResult.Success;
                }
            }

            // For List<IFormFile>
            if (value is List<IFormFile> files)
            {
                if (!files.Any() || files.All(f => f.Length == 0))
                {
                    return ValidationResult.Success;
                }
            }

            return ValidationResult.Success;
        }
    }
} 