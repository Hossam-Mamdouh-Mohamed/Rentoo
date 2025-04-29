using System.ComponentModel.DataAnnotations;
using Rentoo.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Rentoo.Web.ViewModels
{
    public class AddCarViewModel
    {
        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Transmission is required")]
        [StringLength(20, ErrorMessage = "Transmission cannot exceed 20 characters")]
        public string Transmission { get; set; }

        [Required(ErrorMessage = "Number of seats is required")]
        [Range(1, 10, ErrorMessage = "Seats must be between 1 and 10")]
        public int Seats { get; set; }

        [Required(ErrorMessage = "Color is required")]
        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Air condition status is required")]
        public bool AirCondition { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Factory year is required")]
        [Range(1900, 2100, ErrorMessage = "Factory year must be between 1900 and 2100")]
        public int FactoryYear { get; set; }

        [Required(ErrorMessage = "Driver option is required")]
        public WithDriverEnum WithDriver { get; set; }

        [Required(ErrorMessage = "Fuel type is required")]
        [StringLength(30, ErrorMessage = "Fuel type cannot exceed 30 characters")]
        public string Fuel { get; set; }

        [Required(ErrorMessage = "Mileage is required")]
        [Range(0.1f, float.MaxValue, ErrorMessage = "Mileage must be greater than 0")]
        public float Mileage { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        public bool IsAvailable { get; set; }

        [Required(ErrorMessage = "Car images are required")]
        [Display(Name = "Car Images")]
        public List<IFormFile> CarImages { get; set; }

        [Required(ErrorMessage = "License document is required")]
        [Display(Name = "License Document")]
        public IFormFile LicenseUrl { get; set; }

        [Display(Name = "License Number")]
        [Required(ErrorMessage = "License number is required")]
        [RegularExpression(@"^[\p{L}\u0600-\u06FF]{3}(\s?[\p{L}\u0600-\u06FF]){0,2}\s?\d{3,4}$", ErrorMessage = "Plate number should consist of 3 Arabic letters followed by 3 or 4 digits.")]
        public string LicenseNumber { get; set; }
    }
} 