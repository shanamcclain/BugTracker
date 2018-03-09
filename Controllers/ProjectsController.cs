﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {

            return View(db.Projects.ToList());
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
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
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
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
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
        public ActionResult EditUsers([Bind(Include = "Users,Project,SelectedUsers")] ProjectViewModel model)
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
                }
            }

            return RedirectToAction("Index");
        }

    }
}