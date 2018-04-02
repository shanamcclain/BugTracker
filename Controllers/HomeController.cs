using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            Contact model = new Contact();
            return View(model);
        }
        //GET: UserPage
        public ActionResult UserPage()
        {
            UserPageViewModel vm = new UserPageViewModel();
            var userid = User.Identity.GetUserId();
            var allProjects = db.Projects.Where(p => p.Tickets.Select(t => t.AssignedToUserId)
           .Contains(userid) || p.Tickets.Select(t => t.OwnerUserId).Contains(userid) || p.PMID.Contains(userid)).ToList();
            var tickets = db.Tickets.Where(u => u.AssignedToUserId == userid).Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);

            vm.Projects = allProjects.Count();
            vm.Tickets = tickets.Count();
            vm.AllMembers = db.Users.ToList();

            return View(vm);
            //return View();
        }
        //POST: UserPage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserPage(string UserId, HttpPostedFileBase image)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.Find(UserId);

            if (ImageUploadValidator.IsWebFriendlyImage(image))
            {
                var fileName = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                user.ProfilePic = "/Uploads/" + fileName;
            }
            else
            {
                return View();
            }

            db.SaveChanges();
            await ControllerContext.HttpContext.RefreshAuthentication(user);
            return RedirectToAction("UserPage");
        }
        public ActionResult LandingPage()
        {
            return View();
        }
    }
}