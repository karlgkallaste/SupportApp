using Microsoft.EntityFrameworkCore;
using SupportApp.Models;

namespace SupportApp.Data
{
    public class SupportAppContext : DbContext
    {
        public SupportAppContext(DbContextOptions<SupportAppContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Ticket { get; set; }
    }
}