using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Rentoo.Domain.Entities
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Range(0, 120)]
        public int Age { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Phone]
        [Required]
        [StringLength(50)]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numeric characters are allowed.")]
        [MinLength(11)]
        public string PhoneNumber { get; set; } 
        public ICollection<Request> Requests { get; set; } = new List<Request>();
        public ICollection<Car> Cars { get; set; } = new List<Car>();

        [Display(Name = "Request Reviews")]
        public ICollection<RequestReview> RequestReview { get; set; } = new List<RequestReview>();

    }
}