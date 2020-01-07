using System;
using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class ConfirmForgotPassword
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public String Email { get; set; }

        [Required]
        [Display(Name = "Code")]
        public String Code { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public String NewPasword { get; set; }
    }
}
