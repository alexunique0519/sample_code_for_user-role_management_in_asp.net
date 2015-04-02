using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YYPatient.Models
{
    public class RoleViewModels
    {
        
        public string roleId;
        
        [Required]
        public string roleName;
    }
}