using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportApp.Data;
using System;
using System.Linq;
using SupportApp.Models.Tickets;

namespace SupportApp.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SupportAppContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<SupportAppContext>>()))
            {
                context.Database.EnsureCreated();
                
                // Look for any movies.
                if (context.Ticket.Any())
                {
                    return;   // DB has been seeded
                }

                context.Ticket.AddRange(
                    new Ticket("When i click on register nothing happens.","Register issue","KGK",DateTime.Parse("2020-3-11"),2),


                    new Ticket("When i click on createnew i get a error.","Createnew issue","KGK",DateTime.Parse("2020-1-11"), 2),

                    new Ticket("When i click on login nothing happens.","Login issue","KGK",DateTime.Parse("2020-1-21"), 1),

                    new Ticket("When i click on login nothing happens.","Login issue","KGK",DateTime.Parse("2020-4-1"), 1)
                );
                context.SaveChanges();
            }
        }
    }
}