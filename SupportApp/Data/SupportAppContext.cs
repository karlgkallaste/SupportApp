using System.Linq;
using Microsoft.EntityFrameworkCore;
using SupportApp.Models;
using SupportApp.Models.Tickets;

namespace SupportApp.Data
{
    public interface ISupportAppContext
    {
        DbSet<T> Set<T>() where T : class;
        IQueryable<T> Queryable<T>() where T : class;
        T FindById<T>(int id) where T : class;
        T FindByAuthor<T>(string author) where T : class;
        int SaveChanges();
    }
    
    public class SupportAppContext : DbContext, ISupportAppContext
    {
        public SupportAppContext(DbContextOptions<SupportAppContext> options)
            : base(options)
        {
        }

        public DbSet<Ticket> Ticket { get; private set; }
        public IQueryable<T> Queryable<T>() where T : class
        {
            return Set<T>().AsQueryable();
        }

        public T FindById<T>(int id) where T : class
        {
            return Find<T>(id);
        }
        public T FindByAuthor<T>(string author) where T : class
        {
            return Find<T>(author);
        }
    }
}