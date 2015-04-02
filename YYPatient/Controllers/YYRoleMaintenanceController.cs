using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYPatient.Models;

//this controller is for the role maintenance
namespace YYPatient.Controllers
{
    //require anyone using this controller to be signed on
    [Authorize(Roles="administrators")]
    public class YYRoleMaintenanceController : Controller
    {
        public static ApplicationDbContext db = new ApplicationDbContext();
       
        private UserManager<ApplicationUser> userManager =
        new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
       
        
        private RoleManager<IdentityRole> roleManager =
        new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

        //retrieve all the role from database, and put them into a role list. Display all the role in index page.
        public ActionResult Index()
        {
            List<RoleViewModels> roleList= new List<RoleViewModels>();

            if(db.Roles.Count() == 0)
            {
                return View();
            }
            
            foreach (var role in db.Roles)
            {
                RoleViewModels rv = new RoleViewModels();
                rv.roleId = role.Id;
                rv.roleName = role.Name;

                roleList.Add(rv);
            }
            
            return View(roleList.OrderBy(m => m.roleName));
        }

        //get the passed RoleName from the textbox, if the RoleName is valid, then create a new role
        //and display success/fail message on the role listing page.
        [HttpPost]
        public ActionResult Create(string RoleName)
        {
            if(RoleName == null)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "role name cannot be null";
                
                return RedirectToAction("Index");
            }

            RoleName = RoleName.Trim();
            if(RoleName == "")
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "role name cannot be empty or blank";

                return RedirectToAction("Index");
            }


            try 
            {
                //check the role is exist or not.
                bool bExist = roleManager.RoleExists(RoleName);
                if(bExist)
                {
                    TempData["messageType"] = "danger";
                    TempData["message"] = "The role - " + RoleName + " is already on file";

                    return RedirectToAction("Index");
                }
                
                //new a role object and save it into database.
                IdentityRole newRole = new IdentityRole(RoleName);
                IdentityResult ir = roleManager.Create(newRole);

                if (ir.Succeeded)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "The role: " + RoleName + " has been created successfully";
                }
                
                else
                {
                    string errors = null;

                    foreach (var error in ir.Errors)
                    {
                        errors += error;
                    }

                    throw new Exception(errors);
                }

            }
            catch (Exception ex)
            {
                TempData["MessageType"] = "danger";
                TempData["message"] = "Exception creating the role: '" +
                        RoleName + "' – " + ex.GetBaseException().Message;
            }         

            return RedirectToAction("Index");
        }

        //get the passed roleName, if the roleName is valid and not 'administrator', then 
        //find all the users belong to this role, and send them to delete view.
        public ActionResult Delete(string roleName)
        {
            if (roleName == null)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "Please select a valid role";
                return RedirectToAction("Index");
            }

            roleName = roleName.Trim();
            if (roleName == "")
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "Please select a valid role";
                return RedirectToAction("Index");
            }

            if (string.Equals(roleName, "administrators", StringComparison.CurrentCultureIgnoreCase))
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "The 'administrators' role cannot be deleted!";
                return RedirectToAction("Index");
            }

            DeleteRoleViewModels DR = new DeleteRoleViewModels();

            List<UserViewModels> UserList = new List<UserViewModels>();

            foreach(var user in db.Users)
            {
                if(userManager.IsInRole(user.Id, roleName))
                {
                    UserViewModels uv = new UserViewModels();
                    uv.userId = user.Id;
                    uv.userName = user.UserName;
                    uv.email = user.Email;
                    UserList.Add(uv);
                }
                     
            }

            DR.roleName = roleName;
            DR.bRemove  = false;
            DR.roleUserList = UserList;

            return View(DR);
        }


        //retrieve the roleName and value of the checkbox, then start the deleting operation
        [HttpPost]
        public ActionResult Delete(string roleName, FormCollection form)
        {
            bool bRemoveUsers = false;
            List<UserViewModels> userList = new List<UserViewModels>();
            foreach (var user in db.Users)
            {
                if (userManager.IsInRole(user.Id, roleName))
                {
                    UserViewModels uv = new UserViewModels();
                    uv.userId = user.Id;
                    uv.userName = user.UserName;
                    uv.email = user.Email;
                    userList.Add(uv);
                }
            }

            //judge the selected value of the checkbox
            string remove = form["bRemove"];

            if (-1 == remove.IndexOf("true"))
            {
                bRemoveUsers = false;
            }
            else
            {
                bRemoveUsers = true;
               
            }

            try { 
                //if select remove user and userlist is not empty, then remove user from the role first, 
                //then delete the role.
                if (bRemoveUsers && userList.Count() > 0)
                {
                   foreach(var user in userList)
                   {
                      IdentityResult ir = userManager.RemoveFromRole(user.userId, roleName);
                      if (!ir.Succeeded)
                      {
                          string errors = null;

                          foreach (var error in ir.Errors)
                          {
                              errors += error;
                          }

                          throw new Exception(errors);
                
                      }
                        
                   }

                   IdentityRole role = roleManager.FindByName(roleName);
                   IdentityResult deleteRes = roleManager.Delete(role);
                   if (deleteRes.Succeeded)
                   {
                       TempData["MessageType"] = "success";
                       TempData["Message"] = "The role - " + roleName + " has been deleted successfully.";
                       return RedirectToAction("index");
                   }
                   else
                   {
                       string errors = null;

                       foreach (var error in deleteRes.Errors)
                       {
                           errors += error;
                       }

                       throw new Exception(errors);
                   }

                }
                //if not select remove user and userlist is not empty, do not delete the role,instead show a message
                //and return to the page.
                else if (!bRemoveUsers && userList.Count() > 0)
                {
                    TempData["MessageType"] = "danger";
                    TempData["Message"] = "The role - " + roleName + " cannot be deleted, because you did not select the remove user option";
                }
                //if there is no user in the role, delete the role.
                else
                {
                    IdentityRole role = roleManager.FindByName(roleName);
                    IdentityResult deleteRes = roleManager.Delete(role);

                    if (deleteRes.Succeeded)
                    {
                        TempData["MessageType"] = "success";
                        TempData["Message"] = "The role - " + roleName + " has been deleted successfully.";
                        return RedirectToAction("index");
                    }
                    else
                    {
                        string errors = null;

                        foreach (var error in deleteRes.Errors)
                        {
                            errors += error;
                        }

                        throw new Exception(errors);
                    }
                }


            }
            catch(Exception ex)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Exception deleting the role: " +roleName + " " + ex.GetBaseException().Message;
                
            }


            DeleteRoleViewModels DR = new DeleteRoleViewModels();
            DR.roleName = roleName;
            DR.bRemove = bRemoveUsers;
            DR.roleUserList = userList;

            return View(DR);
        }

        //get the roleName and save user in two seperate userlist, which are users in the role and not in the role
        //save the userNotInRole list in viewBag to generate the dropdown list.
        public ActionResult ManageUsersInTheRole(string roleName)
        {
            if (roleName == null)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "Please select a valid role";
                return RedirectToAction("Index");
            }

            roleName = roleName.Trim();
            if (roleName == "")
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "Please select a valid role";
                return RedirectToAction("Index");
            }

            List<UserViewModels> UserList = new List<UserViewModels>();
            List<UserViewModels> UserListNotInRole = new List<UserViewModels>();

            //currenUserId
            string currentUserId = User.Identity.GetUserId();
            //string userName = userManager.FindById(currentUserId).UserName;

            foreach (var user in db.Users)
            {
                //only save the user which in the role and not the current user in the userlist
                if (userManager.IsInRole(user.Id, roleName))
                {
                    UserViewModels uv = new UserViewModels();
                    uv.userId = user.Id;
                    uv.userName = user.UserName;
                    uv.email = user.Email;
                    UserList.Add(uv);
                }
                else
                {
                    UserViewModels uv = new UserViewModels();
                    uv.userId = user.Id;
                    uv.userName = user.UserName;
                    uv.email = user.Email;
                    UserListNotInRole.Add(uv);
                }

            }

            ViewBag.currentUserId = currentUserId;
            ViewBag.roleName = roleName;
            ViewBag.userNotInRoleList = new SelectList(UserListNotInRole, "userId", "userName");
            


            return View(UserList);

        }

        //get the userName and roleName, is both the name are valid. then start to remove the user from the passed role.
        public ActionResult RemoveUser(string userName, string roleName)
        {
            if (userName == null || userName == "" || roleName == null || roleName == "")
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "The user or the role info is not valid";
                return RedirectToAction("ManageUsersInTheRole");
            }
            
            //get the use object from datebase
            ApplicationUser user = userManager.FindByName(userName);
            
            if(user == null)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "The user - " + userName + " is not in database";
                return RedirectToAction("ManageUsersInTheRole");
            }

            try {
                IdentityResult ir = userManager.RemoveFromRole(user.Id, roleName);
                
                if (ir.Succeeded)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Remove the user - " + userName + " from the role " + roleName + " succeed";
                }
                else
                {
                    string errors = null;

                    foreach (var error in ir.Errors)
                    {
                        errors += error;
                    }

                    throw new Exception(errors);
                    
                }            
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "Remove the user - " + userName + " from the role " + roleName + " failed :" +  ex.GetBaseException().Message;
                
            }

            //return to the ManageUsersInTheRole view.
            return RedirectToAction("ManageUsersInTheRole", new { roleName = roleName });
        }

        //get the userId and role name from the submit action, is both the names are valid, then add the user in the passed role.
        [HttpPost]
        public ActionResult AddUser(string userNotInRoleList, string roleName)
        {
            if (userNotInRoleList == null || userNotInRoleList == "")
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "the user name is not valid";

                return RedirectToAction("ManageUsersInTheRole", new { roleName = roleName });
            }

            string userId = userNotInRoleList;

            //
            ApplicationUser user = userManager.FindById(userId);
            try
            {
                IdentityResult ir = userManager.AddToRole(userId, roleName);

                if (ir.Succeeded)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "Add the user - " + user.UserName + " into the role " + roleName + " succeed";
                }
                else
                {
                    string errors = null;

                    foreach (var error in ir.Errors)
                    {
                        errors += error;
                    }

                    throw new Exception(errors);
                }
            }
            catch (Exception ex)
            {
                TempData["messageType"] = "danger";
                TempData["message"] = "Add the user - " + user.UserName + " into the role " + roleName + " failed: " + ex.GetBaseException().Message;

            }

            return RedirectToAction("ManageUsersInTheRole", new { roleName = roleName});
            
        }
        
    }
}