using Microsoft.EntityFrameworkCore;
using Pellio.Models;

namespace Pellio.Data
{
    public class PellioContext : DbContext
    {
        public PellioContext(DbContextOptions<PellioContext> options)
            : base(options)
        {
        }

        public DbSet<Products> Products { get; set; }

        public DbSet<Comments> Comments { get; set; }

        public DbSet<Cart> Cart { get; set; }
    }
}