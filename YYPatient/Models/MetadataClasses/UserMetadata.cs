using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YYPatient.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class UserViewModels
    {

    }

    public class UserMetadata
    {
        public string userId{get; set;}

        [Display(Name = "User Name")]
        public string userName { get; set; }

        [Display(Name = "Email")]
        public string email {get; set;}

        [Display(Name = "Lockout")]
        public bool isLockout{get; set;}

        [Display(Name = "Local Authentication")]
        public bool isAuthenticatedLocally{get;set;}
        
        public bool isAdmin{get; set;}
      
    }
}