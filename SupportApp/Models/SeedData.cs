using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SupportApp.Data;
using System;
using System.Linq;

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
                // Look for any movies.
                if (context.Ticket.Any())
                {
                    return;   // DB has been seeded
                }

                context.Ticket.AddRange(
                    new Ticket
                    {
                        Title = "Login issue",
                        Description = "When i click on login nothing happens.",
                        Author = "KGK",
                        CreatedAt = DateTime.Parse("2020-3-11"),
                    },

                    new Ticket
                    {
                        Title = "Problem with my payment",
                        Description = "The money was deducted from my bank account but my site balance is still the same.",
                        Author = "Karl",
                        CreatedAt = DateTime.Parse("2020-2-1"),
                    },

                    new Ticket
                    {
                        Title = "Forgot my login info",
                        Description = "I don't remeber my login info and i don't have access to the email i used.",
                        Author = "Gus",
                        CreatedAt = DateTime.Parse("2020-1-22"),
                    },

                    new Ticket
                    {
                        Title = "Account got hacked",
                        Description = "Someone has changed my passowrd.",
                        Author = "Carl",
                        CreatedAt = DateTime.Parse("2019-5-31"),
                    }
                );
                context.SaveChanges();
            }
        }
    }
}