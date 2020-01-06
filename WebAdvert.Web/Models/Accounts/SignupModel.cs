using System;
using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class SignupModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public String Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(8, ErrorMessage = "Password must be at least eight characters long, have letter, have numbers and special characters!")]
        [Display(Name ="Password")]
        public String Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password and its confirmation do not match")]
        public String ConfitmPassword { get; set; }

    }
}
