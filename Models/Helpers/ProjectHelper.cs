using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models.Helpers
{
    public class ProjectHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
       
        public bool AddUserToProject(string UserId, int ProjectId)
        { 
            try
            {
                var prj = db.Projects.Find(ProjectId);
                var usr = db.Users.Find(UserId);
                prj.Users.Add(usr);
                db.SaveChanges();
                return true;

            }
            catch
            {
                return false;
            }
        }
        public bool RemoveUserFromProject(string UserId, int ProjectId)
        {
            try
            {
                var prj = db.Projects.Find(ProjectId);
                var usr = db.Users.Find(UserId);
                prj.Users.Remove(usr);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public ICollection<Project> ListUserProjects(string UserId)
        {
            return db.Users.Find(UserId).Projects.ToList();
        }
        public bool IsUserOnProject(string UserId, int ProjectId)
        {
            try
            {
                var usr = db.Users.Find(UserId);
                var result = db.Projects.Find(ProjectId).Users.Contains(usr);
                return result;
            }
            catch
            {
                return false;
            }
        }
        public ICollection<Project> ListUsersOnProject(string ProjectId)
        {
            return db.Users.Find(ProjectId).Projects.ToList();
        }
        public ICollection<ApplicationUser> ListUsersNotInProject(int projectId)
        {
            List<ApplicationUser> usrNotInProject = new List<ApplicationUser>();
            List<ApplicationUser> usrs = db.Users.ToList();

            foreach (var u in usrs)
            {
                if (!IsUserOnProject(u.Id, projectId))
                {
                    usrNotInProject.Add(u);
                }
            }
            return usrNotInProject;
        }

    }
}