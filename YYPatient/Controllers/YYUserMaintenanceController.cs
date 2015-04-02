using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YYPatient.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;

namespace YYPatient.Controllers
{

    [Authorize(Roles = "administrators")]
    public class YYUserMaintenanceController : Controller
    {
       public ApplicationDbContext db = new ApplicationDbContext();
 /*        private UserManager<ApplicationUser> userManager =
          new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
         private RoleManager<IdentityRole> roleManager =
            new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
*/ 
       
        private ApplicationUserManager _userManager;
        public YYUserMaintenanceController()
        {
            
        }

        public YYUserMaintenanceController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
            
        }

        public ApplicationUserManager UserManager {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
 
        
        //First, get all the user from the database, then in the foreach loop, generate userviewmodel object and 
        //assign value for them, and put them into a list, which will be passed to the index view.
        public ActionResult Index()
        {
            List<UserViewModels> Userlist = new List<UserViewModels>();

            foreach (ApplicationUser user in db.Users)
            {
                UserViewModels uv = new UserViewModels();
                uv.userId = user.Id;
                uv.userName = user.UserName;
                uv.email = user.Email;
                uv.isLockout = user.LockoutEnabled;

                var Logins = UserManager.GetLogins(user.Id).ToList();

                if (Logins.Count > 0)
                {
                    uv.isAuthenticatedLocally = false;
                }

                Boolean isAdministrator = UserManager.IsInRole(uv.userId, "administrators");

                uv.isAdmin = isAdministrator;

                Userlist.Add(uv);
            }

            int count = Userlist.Count();

            //sort the userlist by lockout status first, then username
            return View(Userlist.OrderByDescending(m => m.isLockout).ThenBy(m => m.userName));
        }

        //first, check whether the passed userId is valid or not.If the userId is valid find the user object
        public ActionResult DeleteUser(string id)
        {
            if(id == null || id == "")
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Please select a valid user.";

                return RedirectToAction("Index", "YYUserMaintenance");
            }

            ApplicationUser user = UserManager.FindById(id);
            IdentityResult ir = IdentityResult.Success;

            try
            {
                if (user != null)
                {
                    //get all the roles that the user belongs to
                    var UserRoles = UserManager.GetRoles(user.Id).ToList();
                    foreach(string role in UserRoles)
                    {
                        //remove user from each role
                        ir = UserManager.RemoveFromRole(user.Id, role);
                        //if failed
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
                    
                    // after remove from all the roles, delete the user
                    ir = UserManager.Delete(user);
                    if(ir.Succeeded)
                    {
                        TempData["MessageType"] = "success";
                        TempData["Message"] = "The user - " + user.UserName + " has been deleted successfully";
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
            }
            catch (Exception ex)
            {
                TempData["MessageType"] = "danger";
                TempData["message"] = "Exception deleting user '" + 
                        user.UserName + "' – " + ex.GetBaseException().Message;
            }

            return RedirectToAction("Index");
        }

        public ActionResult ResetPassword(string id)
        {
            if(id == null || id == "")
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Please select a valid user.";

                return RedirectToAction("Index", "YYUserMaintenance");
            }

            ApplicationUser user = UserManager.FindById(id);
            if(user == null)
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "This user is not a valid user";

                return RedirectToAction("Index", "YYUserMaintenance");
            }

            //check the role of the user, if the role is administrators, then return to the index listing and show a message
            if (UserManager.IsInRole(user.Id, "administrators"))
            {
                TempData["MessageType"] = "danger";
                TempData["Message"] = "Cannot reset the password for users who are in the role of 'administrator'";

                return RedirectToAction("Index", "YYUserMaintenance");
            }

            ResetPassword rp = new ResetPassword();
            rp.userId = user.Id;
            rp.userName = user.UserName;
            
            string passwordToken = UserManager.GeneratePasswordResetToken(rp.userId);

            rp.token = passwordToken;
            

            return View(rp);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPassword rp)
        {
            if (rp == null)
            {
                ModelState.AddModelError("", "The operation is not valid");
                return View();
            }

            var user = UserManager.FindById(rp.userId);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found.");
                return View();
            }
            
            try
            {
                if (ModelState.IsValid)
                {
                    IdentityResult ir = UserManager.ResetPassword(rp.userId, rp.token, rp.newPassword);

                    if(ir.Succeeded)
                    {
                        TempData["messageType"] = "success";
                        TempData["message"] = "The password is reset successfully.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in ir.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.GetBaseException().Message);
            }          
            
            return View(rp);

        }

      
    
        public ActionResult changeStatus(string id, bool bLock)
        {
            if(id == null || id == "")
            {
                TempData["messageType"] = "success";
                TempData["message"] = "The user you selected is not valid";
                return RedirectToAction("index");
            }

            try
            {
                ApplicationUser user = UserManager.FindById(id);
                IdentityResult ir = UserManager.SetLockoutEnabled(id, !bLock);
                
                if (ir.Succeeded)
                {
                    TempData["messageType"] = "success";
                    TempData["message"] = "User: " + user.UserName + " is " + (bLock ? "Unlocked" : "Locked");
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
                TempData["message"] = "Exception thrown: " + ex.GetBaseException().Message;
            }

            return RedirectToAction("Index");
        }

      

    }
}


