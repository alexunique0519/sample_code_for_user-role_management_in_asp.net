using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YYPatient.Models
{
    public class DeleteRoleViewModels
    {
        public string roleName;
        public List<UserViewModels> roleUserList;
        public bool bRemove;
    }
}