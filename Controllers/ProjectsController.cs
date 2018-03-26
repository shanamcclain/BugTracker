using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> userManager =
           new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        // GET: Projects
        public ActionResult Index()
        {
            List<PMViewModel> pm = new List<PMViewModel>();
            foreach(var proj in db.Projects.ToList())
            {
                PMViewModel pmvm = new PMViewModel();
                pmvm.Project = proj;
                pmvm.PM = db.Users.Find(proj.PMID);
                pm.Add(pmvm);
            }

            return View(pm);
        }

        // GET: MyProjects
        public ActionResult MyProjects()
        {
            var userid = User.Identity.GetUserId();
            //var tickets = db.Tickets.Where(u => u.AssignedToUserId == userid);
            //var allTickets = db.Projects.Where(p => p.PMID == userid).SelectMany(t => t.Tickets).ToList();
            //var allTickets = db.Projects.Where(p => p.Users.Select(u=>u.Id).Contains(userid)).SelectMany(t => t.Tickets).ToList();
            //var tickets = db.Tickets.Where(t => t.AssignedToUserId == userid || t.OwnerUserId == userid).ToList();

            var allProjects = db.Projects.Where(p => p.Tickets.Select(t => t.AssignedToUserId)
            .Contains(userid) || p.Tickets.Select(t => t.OwnerUserId).Contains(userid)).ToList();

            return View(allProjects);
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            var userid = User.Identity.GetUserId();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Projects.Find(id).Users.Select(u => u.Id).Contains(userid))
            {
                Project project = db.Projects.Find(id);
                return View(project);
            }
            else if (!db.Projects.Find(id).Users.Select(u => u.Id).Contains(userid))
            {
                return HttpNotFound();
            }
            return View();
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create/
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //GET: EditUsers
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult EditUsers(int? id)
        {
            ProjectViewModel vm = new ProjectViewModel();
            var prj = db.Projects.Find(id);
            vm.Project = prj;
            var selected = prj.Users.ToList();
            vm.Users = new MultiSelectList(db.Users, "Id", "FirstName", selected);
            return View(vm);
        }

        //POST: EditUsers
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ProjectManager")]
        public async Task<ActionResult> EditUsers([Bind(Include = "Users,Project,SelectedUsers")] ProjectViewModel model, TicketNotification ticketNotification)
        {
            ProjectHelper helper = new ProjectHelper();
            foreach (var user in db.Users)
            {
                if (helper.IsUserOnProject(user.Id, model.Project.Id))
                {
                    helper.RemoveUserFromProject(user.Id, model.Project.Id);
                }
            }
            foreach (var user in model.SelectedUsers)
            {
                if (!helper.IsUserOnProject(user, model.Project.Id))
                {
                    helper.AddUserToProject(user, model.Project.Id);
                    //userManager.SendEmailAsync(user, "Notification", "You have been added to project " + model.Project.Name);
                    var callbackUrl = Url.Action("Details", "Projects", new { id = model.Project.Id }, protocol: Request.Url.Scheme);
                    try
                    {
                        EmailService ems = new EmailService();
                        IdentityMessage msg = new IdentityMessage();
                        ApplicationUser usr = db.Users.Find(user);
                        msg.Body = "You have been assigned a new Ticket." + Environment.NewLine + "Please click the following link to view the details" + "<a href=\"" + callbackUrl + "\">NEW TICKET</a>";
                        msg.Destination = usr.Email;
                        msg.Subject = "BugTracker";
                        await ems.SendMailAsync(msg);
                    }
                    catch (Exception ex)
                    {
                        await Task.FromResult(0);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult AssignPm(int? id)
        {
            UserRolesHelper helper = new UserRolesHelper();
            var project = db.Projects.Find(id);
            var users = helper.UsersInRole("ProjectManager").ToList();
            ViewBag.PMID = new SelectList(users, "Id", "FirstName", project.PMID);
            return View(project);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignPm(Project model)
        {
            var project = db.Projects.Find(model.Id);
            project.PMID = model.PMID;
            db.SaveChanges();

            var callbackUrl = Url.Action("Details", "Projects", new { id = project.Id }, protocol: Request.Url.Scheme);
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser user = db.Users.Find(model.PMID);
                msg.Body = "You have been assigned a new Project." + Environment.NewLine + "Please click the following link to view the details  " + "<a href=\"" + callbackUrl + "\">NEW PROJECT</a>";
                msg.Destination = user.Email;
                msg.Subject = "BugTracker";
                await ems.SendMailAsync(msg);
            }
            catch (Exception ex) { await Task.FromResult(0); }
            return RedirectToAction("Index");
        }

    }
}