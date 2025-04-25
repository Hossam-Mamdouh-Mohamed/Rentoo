using Rentoo.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Rentoo.Web.FiltersAndCustomValidation;

namespace Rentoo.Web.ViewModels
{
    [ValidDateRange]
    public class RequestViewModel
    {

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "From")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "To")]
        public DateTime EndDate { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Total Price")]
        [Column(TypeName = "decimal(18,2)")]
        public float TotalPrice { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Address must be at least 5 characters long.")]
        [StringLength(200)]    
        public string DeliveryAddress { get; set; }

        [Required]
        [StringLength(200)]
        [MinLength(5, ErrorMessage = "Address must be at least 5 characters long.")]
        public string pickupAddress { get; set; }


        [Required]
        [Display(Name = "With Driver")]
        public bool WithDriver { get; set; }

    }
}
