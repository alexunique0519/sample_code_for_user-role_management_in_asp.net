using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YYPatient.Models
{
    [MetadataType(typeof(ResetPasswordMetadata))]
    public partial class ResetPassword
    {

    }


    public class ResetPasswordMetadata
    {
        public string userId;

        [Display(Name = "Name")]
        public string userName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string newPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("newPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string confirmPassword { get; set; }

        public string token { get; set; }

    }



}