using System.ComponentModel.DataAnnotations;

namespace web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Role { get; set; } // Add this property
        public string PhoneNumber { get; set; }
    }
}