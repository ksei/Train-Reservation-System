namespace TrainReservation.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TrainReservation.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "TrainReservation.Models.ApplicationDbContext";
        }

        protected override void Seed(TrainReservation.Models.ApplicationDbContext context)
        {
            if (!context.Users.Any(u => u.UserName == "Ksandros"))
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = new ApplicationUser { UserName = "Ksadnros" };
                userManager.Create(user, "ksei");
                roleManager.Create(new IdentityRole { Name = "admin" });
                userManager.AddToRole(user.Id, "admin");
            }
        }
    }
}