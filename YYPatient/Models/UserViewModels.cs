using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YYPatient.Models
{
    public partial class UserViewModels
    {
        public string userId{get; set;}

        public string userName { get; set; }
       
        public string email {get; set;}

        public bool isLockout{get; set;}
      
        public bool isAuthenticatedLocally{get;set;}
        
        public bool isAdmin{get; set;}
       
        public UserViewModels()
        {
            this.isAuthenticatedLocally = true;
            this.isAdmin = false;
        }
    
    }
}