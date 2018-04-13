using System;
using System.Collections.Generic;
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

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        // GET: MyTickets
        public ActionResult MyTickets()
        {
            var userid = User.Identity.GetUserId();
            var tickets = db.Tickets.Where(u => u.AssignedToUserId == userid).Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }

        // GET: OwnedTickets
        public ActionResult OwnedTickets()
        {
            var userid = User.Identity.GetUserId();
            var tickets = db.Tickets.Where(u => u.OwnerUserId == userid).Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(tickets.ToList());
        }


        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        ////POST: Tickets/Details/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Details()
        //{

        //}

        // GET: Tickets/Create
        [Authorize(Roles = "Admin,Submitter")]
        public ActionResult Create()
        {
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Submitter")]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = DateTime.Now;
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.TicketStatusId = 1;
                ticket.TicketPriorityId = 4;
                ticket.AssignedToUserId = db.Users.FirstOrDefault(n => n.FirstName == "Unassigned").Id;
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            //ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            //ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            //ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin,ProjectManager,Developer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        [Authorize(Roles = "Admin,ProjectManager,Developer")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                //bool ticketAssigned = false;
                foreach (var prop in typeof(Ticket).GetProperties())
                {
                    if (prop.Name != null && prop.Name.In("Title", "Description", "TicketTypeId", "TicketPriorityId", "TicketStatusId", "AssignedToUserId"))
                    {
                        var oldPropId = oldTicket.GetType().GetProperty(prop.Name).GetValue(oldTicket);
                        var newPropId = ticket.GetType().GetProperty(prop.Name).GetValue(ticket);

                        var oldValue = oldTicket.GetType().GetProperty(prop.Name).GetValue(oldTicket).ToString();
                        var newValue = ticket.GetType().GetProperty(prop.Name).GetValue(ticket).ToString();

                        if (prop.Name == "TicketTypeId")
                        {
                            oldValue = db.TicketTypes.Find(oldPropId).Name;
                            newValue = db.TicketTypes.Find(newPropId).Name;
                        }
                        if (prop.Name == "TicketStatusId")
                        {
                            oldValue = db.TicketStatuses.Find(oldPropId).Name;
                            newValue = db.TicketStatuses.Find(newPropId).Name;
                        }
                        if (prop.Name == "TicketPriorityId")
                        {
                            oldValue = db.TicketPriorities.Find(oldPropId).Name;
                            newValue = db.TicketPriorities.Find(newPropId).Name;
                        }
                        //if (prop.Name == "AssignedToUserId")
                        //{
                        //    if (oldValue != newValue)
                        //    {
                        //        ticketAssigned = true;
                        //        TicketEmail(ticket, newPropId.ToString());
                        //    }
                        //    oldValue = db.Users.Find(oldPropId).DisplayName;
                        //    newValue = db.Users.Find(newPropId).DisplayName;
                        //}
                        if (oldValue != newValue)
                        {
                            TicketHistory ticketHistory = new TicketHistory()
                            {
                                TicketId = oldTicket.Id,
                                UserId = User.Identity.GetUserId(),
                                Property = prop.Name,
                                OldValue = oldValue,
                                NewValue = newValue,
                                Changed = DateTime.Now
                            };
                            db.Histories.Add(ticketHistory);
                        }

                    }
                }
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Details", new { id = ticket.Id });
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
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

        // GET: Tickets/EditPm
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult EditPm(int? id)
        {
            UserRolesHelper urh = new UserRolesHelper();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignedToUserId = new SelectList(urh.UsersInRole("Developer"), "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/EditPm
        [Authorize(Roles = "Admin,ProjectManager")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPm([Bind(Include = "Id,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                //var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);



                //        var oldPropId = oldTicket.Id;
                //        var newPropId = ticket.GetType().GetProperty(prop.Name).GetValue(ticket);

                //        var oldValue = oldTicket.GetType().GetProperty(prop.Name).GetValue(oldTicket).ToString();
                //        var newValue = ticket.GetType().GetProperty(prop.Name).GetValue(ticket).ToString();

                var tick = db.Tickets.Find(ticket.Id);
                tick.AssignedToUserId = ticket.AssignedToUserId;

                db.SaveChanges();

                //Send Dev an email
                await TicketEmail(ticket, tick.AssignedToUserId);


                return RedirectToAction("Index");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        private async Task<bool> TicketEmail(Ticket ticket, string userId)
        {
            var callbackUrl = Url.Action("Details", "Tickets", new { id = ticket.Id }, protocol: Request.Url.Scheme);
            try
            {
                EmailService ems = new EmailService();
                IdentityMessage msg = new IdentityMessage();
                ApplicationUser usr = db.Users.Find(userId);
                msg.Body = "You have been assigned a Ticket." + Environment.NewLine + "Please click the following link to view the details" + "<a href=\"" + callbackUrl + "\">NEW TICKET</a>";
                msg.Destination = usr.Email;
                msg.Subject = "BugTracker";
                await ems.SendMailAsync(msg);
                return true;
            }
            catch (Exception ex)
            {
                await Task.FromResult(0);
                return false;
            }
        }
    }
}
