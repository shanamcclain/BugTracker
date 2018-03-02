namespace BugTracker.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using BugTracker.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //if (!System.Diagnostics.Debugger.IsAttached)
            //    System.Diagnostics.Debugger.Launch();
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            var role = new IdentityRole();

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                role = new IdentityRole { Name = "Admin" };
                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                role = new IdentityRole { Name = "Developer" };
                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                role = new IdentityRole { Name = "Submitter" };
                manager.Create(role);
            }

            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                role = new IdentityRole { Name = "ProjectManager" };
                manager.Create(role);
            }

            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);

            if (!context.Users.Any(u => u.Email == "shanamcclain7@gmail.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "shanamcclain7@gmail.com",
                    Email = "shanamcclain7@gmail.com",
                    FirstName = "Shana",
                    LastName = "Sanders",
                    DisplayName = "Shana"
                };

                userManager.Create(user, "Mcclain1!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "Admin"
                    });
            }
            if (!context.Users.Any(u => u.Email == "manager@email.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "manager@email.com",
                    Email = "manager@email.com",
                    FirstName = "Manager",
                    LastName = "Role",
                    DisplayName = "MANGR"
                };

                userManager.Create(user, "Mcclain1!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "ProjectManager"
                    });
            }
            if (!context.Users.Any(u => u.Email == "developer@email.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "developer@email.com",
                    Email = "developer@email.com",
                    FirstName = "Developer",
                    LastName = "Role",
                    DisplayName = "DEVPR"
                };

                userManager.Create(user, "Mcclain1!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "Developer"
                    });
            }
            if (!context.Users.Any(u => u.Email == "submitter@email.com"))
            {
                var user = new ApplicationUser
                {
                    UserName = "submitter@email.com",
                    Email = "submitter@email.com",
                    FirstName = "Submitter",
                    LastName = "Role",
                    DisplayName = "SUBMT"
                };

                userManager.Create(user, "Mcclain1!");

                userManager.AddToRoles(user.Id,
                    new string[] {
                        "Submitter"
                    });
            }


            if (!context.TicketPriorities.Any(u => u.Name == "High"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "High" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "Medium"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Medium" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "Low"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Low" }); }

            if (!context.TicketPriorities.Any(u => u.Name == "Urgent"))
            { context.TicketPriorities.Add(new TicketPriority { Name = "Urgent" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Front End"))
            { context.TicketTypes.Add(new TicketType { Name = "Front End" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Back End"))
            { context.TicketTypes.Add(new TicketType { Name = "Back End" }); }

            if (!context.TicketTypes.Any(u => u.Name == "Database"))
            { context.TicketTypes.Add(new TicketType { Name = "Database" }); }

            if (!context.TicketStatuses.Any(u => u.Name == "New"))
            { context.TicketStatuses.Add(new TicketStatus { Name = "New" }); }

            if (!context.TicketStatuses.Any(u => u.Name == "In Development"))
            { context.TicketStatuses.Add(new TicketStatus { Name = "In Development" }); }

            if (!context.TicketStatuses.Any(u => u.Name == "Completed"))
            { context.TicketStatuses.Add(new TicketStatus { Name = "Completed" }); }

        }
    }

}
