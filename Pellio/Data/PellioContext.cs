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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PercentOffCode>()
                .HasOne(a => a.OrdersList)
                .WithOne(b => b.PercentOffCode)
                .HasForeignKey<OrdersList>(b => b.PercentOffCodeId);
            //modelBuilder.Entity<OrdersList>()
            //    .HasOne(a => a.PercentOffCode)
            //    .WithOne(b => b.OrdersList)
            //    .HasForeignKey<PercentOffCode>(b => b.OrdersListId);
        }

        public DbSet<Products> Products { get; set; }

        public DbSet<Comments> Comments { get; set; }

        public DbSet<OrdersList> OrdersList { get; set; }

        public DbSet<EmailCredentials> EmailCredentials { get; set; }
        public DbSet<MadeOrder> MadeOrder { get; set; }
        public DbSet<PercentOffCode> PercentOffCodes { get; set; }
        //public DbSet<Ingredient> Ingredients { get; set; }
    }
}