using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportApp.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SupportApp.Areas.Identity.Data;
using SupportApp.Migrations;
using SupportApp.Models.Categories;
using SupportApp.Models.Tickets;

namespace SupportApp.Models
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.GetRequiredService<SupportAppContext>())
            {
                context.Database.EnsureCreated();
                serviceProvider.GetRequiredService<SupportAppUserContext>().Database.Migrate();
                context.Database.Migrate();
                
                // Look for any movies.
                if (context.Ticket.Any())
                {
                    return;   // DB has been seeded
                }

                await context.Categories.AddAsync(new Category("Category 1"));
                await context.Categories.AddAsync(new Category("Some Other Category 2"));
                await context.SaveChangesAsync();

                var categories = context.Categories.ToArray();

              /*  await context.Ticket.AddRangeAsync(
                    new Ticket("When i click on register nothing happens.","Register issue",new Guid("1"),DateTime.Parse("2020-3-11"), categories[0].Id),


                    new Ticket("When i click on createnew i get a error.","Createnew issue",new Guid("1"),DateTime.Parse("2020-1-11"), categories[0].Id),

                    new Ticket("When i click on login nothing happens.","Login issue",new Guid("1"),DateTime.Parse("2020-1-21"), categories[1].Id),

                    new Ticket("When i click on login nothing happens.","Login issue",new Guid(1),DateTime.Parse("2020-4-1"), categories[1].Id)
                );*/
                await context.SaveChangesAsync();
                var roleManager = serviceProvider.GetService<RoleManager<SupportAppRole>>();
                await roleManager.CreateAsync(new SupportAppRole() {Name = "User"});
                await roleManager.CreateAsync(new SupportAppRole() {Name = "Admin"});

                var userManager = serviceProvider.GetService<UserManager<SupportAppUser>>();
                var admin = new SupportAppUser
                {
                    UserName = "admin@supportapp.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Password1234!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}