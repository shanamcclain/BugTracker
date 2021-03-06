﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models.Helpers
{
    public class UserRolesHelper
    {
        private UserManager<ApplicationUser> userManager =
            new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsUserInRole(string UserId, string Role)
        {
            try
            {
                return userManager.IsInRole(UserId, Role);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        
        public bool AddUserToRole(string UserId, string Role)
        {
            try
            {
                var result = userManager.AddToRole(UserId, Role);
                db.SaveChanges();
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }

       
        public bool RemoveUserFromRole(string UserId, string Role)
        {
            try
            {
                var result = userManager.RemoveFromRole(UserId, Role);
                db.SaveChanges();
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }

        public ICollection<ApplicationUser> UsersInRole(string role)
        {
            List<ApplicationUser> roleUsers = new List<ApplicationUser>();
            List<ApplicationUser> users = userManager.Users.ToList();

            foreach (var u in users)
            {
                if (IsUserInRole(u.Id, role))
                {
                    roleUsers.Add(u);
                }
            }

            return roleUsers;
        }

        public ICollection<ApplicationUser> UsersNotInRole(string role)
        {
            List<ApplicationUser> roleUsers = new List<ApplicationUser>();
            List<ApplicationUser> users = userManager.Users.ToList();

            foreach (var u in users)
            {
                if (!IsUserInRole(u.Id, role))
                {
                    roleUsers.Add(u);
                }
            }

            return roleUsers;
        }

        public ICollection<string> ListUserRoles(string userId)
        {
            try
            {
                return userManager.GetRoles(userId);
            }

            catch
            {
                return null;
            }
        }
    }
}