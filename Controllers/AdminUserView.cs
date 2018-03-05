using BugTracker.Models;
using BugTracker.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using System.Data.Entity;

namespace BugTracker.Controllers
{
    public class AdminUserView : Controller
    {
        ////GET: EditUser
        //public ActionResult EditUser(string id)
        //{
        //    var user = db.Users.Find(id);
        //    AdminUserViewModel AdminModel = new AdminUserViewModel();
        //    UserRolesHelper helper = new UserRolesHelper();
        //    var selected = helper.ListUserRoles(id);
        //    AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
        //    AdminModel.Id= user.Id;
        //    AdminModel.Name = user.DisplayName;
        //    return View(AdminModel);}

        ////POST:  EditUser
        //public ActionResult EditUser(AdminUserViewModel model)
        //{
        //    var user = db.Users.Find(model.id);
        //    UserRolesHelper helper = new UserRolesHelper();
        //    foreach (var rolermv in db.Roles.Select(r => r.Id).ToList())
        //    {
        //        helper.RemoveUserFromRole(user.Id, rolermv);
        //    }
        //    foreach (var roleadd in db.Roles.Select(r => r.Id).ToList())
        //    {
        //        helper.AddUserToRole(user.Id, roleadd);
        //    }
        //    return RedirectToAction("Index");}

    }
}