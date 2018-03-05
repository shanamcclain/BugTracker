using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        [Display(Name = "UserRoles")]
        public string UserRoles { get; set; }

        //navigation property
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<ApplicationUser> User { get; set; }

        public Project()
        {
            User = new HashSet<ApplicationUser>();
            Tickets = new HashSet<Ticket>();
        }
    }
}