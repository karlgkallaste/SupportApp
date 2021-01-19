using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportApp.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SupportApp.Areas.Identity.Data;
using SupportApp.Models.Tickets;

namespace SupportApp.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetService<SupportAppContext>();
            context.Database.EnsureCreated();
            var identityContext = serviceProvider.GetService<SupportAppUserContext>();
            identityContext.Database.Migrate();

            // Look for any movies.
            if (context.Ticket.Any())
            {
                return; // DB has been seeded
            }

            context.Ticket.AddRange(
                new Ticket("When i click on register nothing happens.", "Register issue", "KGK",
                    DateTime.Parse("2020-3-11")),
                new Ticket("When i click on createnew i get a error.", "Createnew issue", "KGK",
                    DateTime.Parse("2020-1-11")),
                new Ticket("When i click on login nothing happens.", "Login issue", "KGK", DateTime.Parse("2020-1-21")),
                new Ticket("When i click on login nothing happens.", "Login issue", "KGK", DateTime.Parse("2020-4-1"))
            );
            await context.SaveChangesAsync();

            var roleManager = serviceProvider.GetService<RoleManager<SupportAppRole>>();
            await roleManager.CreateAsync(new SupportAppRole() {Name = "User"});
            await roleManager.CreateAsync(new SupportAppRole() {Name = "Admin"});

            var userManager = serviceProvider.GetService<UserManager<SupportAppUser>>();
            var user = new SupportAppUser
            {
                UserName = "admin@test.test",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, "Password123!");
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}