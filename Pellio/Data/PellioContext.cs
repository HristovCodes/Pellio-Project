using Microsoft.EntityFrameworkCore;
using Pellio.Models;

namespace Pellio.Data
{
    public class PellioContext : DbContext
    {
        public PellioContext(DbContextOptions<PellioContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Products> Products { get; set; }

        public DbSet<Comments> Comments { get; set; }

        public DbSet<OrdersList> OrdersList { get; set; }

        public DbSet<EmailCredentials> EmailCredentials { get; set; }
        public DbSet<MadeOrder> MadeOrder { get; set; }
    }
}