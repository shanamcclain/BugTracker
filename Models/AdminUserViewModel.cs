using BugTracker.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class AdminUserViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }

    }

    public class UserRolesViewModel
    {
        public ApplicationUser User { get; set; }
        public ICollection<string> Roles { get; set; }
    }

    public class ProjectViewModel
    {
        public Project Project { get; set; }
        public MultiSelectList Users { get; set; }
        public List<string> SelectedUsers { get; set; }

    }

    public class TicketProjectsViewModel
    {
        public List<Project> Projects { get; set; }

    }

    public class UserPageViewModel
    {
        public string ProfilePic { get; set; }
        public string UserId { get; set; }
        public int Tickets { get; set; }
        public int Projects { get; set; }
        public List<ApplicationUser> AllMembers { get; set; }
    }

    public class PMViewModel
    {
        public Project Project { get; set; }
        public ApplicationUser PM { get; set; }

    }

    public class ImageUploadValidator
    {
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;
            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            try
            {
                using (var img = Image.FromStream(file.InputStream))
                {
                    return ImageFormat.Jpeg.Equals(img.RawFormat) ||
                           ImageFormat.Png.Equals(img.RawFormat) ||
                           ImageFormat.Gif.Equals(img.RawFormat);
                }

            }
            catch
            {
                return false;
            }
        }
    }

    public class AdminViewModel
    {
        public List<Ticket> Tickets { get; set; }
        public List<UserRolesViewModel> URVM { get; set; }
        public List<PMViewModel> PVM { get; set; }
    }

    public class Contact
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
        public string Phone { get; set; }
        [Required]
        public DateTime Created { get; set; }
    }

}