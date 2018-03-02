using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
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