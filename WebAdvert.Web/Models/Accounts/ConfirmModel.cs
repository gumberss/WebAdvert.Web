using System;
using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models.Accounts
{
    public class ConfirmModel
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [EmailAddress]
        public String Email { get; set; }

        [Required(ErrorMessage = "Code is required")]

        public String Code { get; set; }

    }
}
