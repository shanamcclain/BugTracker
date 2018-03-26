using BugTracker.Models;
using BugTracker.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using System.Data.Entity;


namespace BugTracker.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        [Authorize(Roles = "Admin")]
        public ActionResult AdminIndex()
        {
            List<UserRolesViewModel> model = new List<UserRolesViewModel>();
            UserRolesHelper helper = new UserRolesHelper();

            var users = db.Users.ToList();
            foreach (var u in users)
            {
                var urvm = new UserRolesViewModel();
                urvm.User = u;
                urvm.Roles = helper.ListUserRoles(u.Id);
                model.Add(urvm);
            }
            return View(model);
        }
        //GET: EditUser
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string id)
        {
            var user = db.Users.Find(id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            UserRolesHelper helper = new UserRolesHelper();
            var selected = helper.ListUserRoles(id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = new ApplicationUser();
            AdminModel.User.Id = user.Id;
            AdminModel.User.DisplayName = user.DisplayName;
            return View(AdminModel);
        }

        //POST:  EditUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser([Bind(Include = "User,Roles,SelectedRoles")] AdminUserViewModel model)
        {
            var user = db.Users.Find(model.User.Id);
            UserRolesHelper helper = new UserRolesHelper();
            foreach (var role in db.Roles.Select(r => r.Name).ToList())
            {
                if (helper.IsUserInRole(user.Id, role))
                {
                    helper.RemoveUserFromRole(user.Id, role);
                }
            }
            foreach (var roleadd in model.SelectedRoles)
            {
                if (!helper.IsUserInRole(user.Id, roleadd))
                {
                    helper.AddUserToRole(user.Id, roleadd);
                }
            }

            return RedirectToAction("AdminIndex");
        }
        // GET: Projects
        public ActionResult AllProjects()
        {
            AdminViewModel avm = new AdminViewModel();
            //Populate project/pm view model
            List<PMViewModel> pm = new List<PMViewModel>();
            foreach (var proj in db.Projects.ToList())
            {
                PMViewModel pmvm = new PMViewModel();
                pmvm.Project = proj;
                pmvm.PM = db.Users.Find(proj.PMID);
                pm.Add(pmvm);
            }
            avm.PVM = pm;
            //Populate tickets view model
            avm.Tickets = db.Tickets.ToList();
            //Populate 
            List<UserRolesViewModel> model = new List<UserRolesViewModel>();
            UserRolesHelper helper = new UserRolesHelper();

            var users = db.Users.ToList();
            foreach (var u in users)
            {
                var urvm = new UserRolesViewModel();
                urvm.User = u;
                urvm.Roles = helper.ListUserRoles(u.Id);
                model.Add(urvm);
            }
            avm.URVM = model;

            return View(db.Projects.ToList());
        }
        // GET: Tickets
        public ActionResult AllTickets()
        {
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

    }
}