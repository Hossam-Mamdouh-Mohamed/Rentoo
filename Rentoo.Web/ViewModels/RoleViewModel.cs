using System.ComponentModel.DataAnnotations;
namespace web.ViewModels
{
    public class RoleViewModel
    {
        [Required(ErrorMessage = "Please Enter The RoleName")]
        [Display(Name = "Role Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string RoleName { get; set; }
    }
}