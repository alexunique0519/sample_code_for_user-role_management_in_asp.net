using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YYPatient.Models
{
    public partial class ResetPassword
    {
        public string userId {get; set;}
        public string userName{get; set;}
        public string newPassword{get; set;}
        public string confirmPassword { get; set; }
        public string token { get; set; }

    }
}