using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pellio.Models;
using Pellio.ViewModels;

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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Products>()
                .HasMany(p => p.Comments)
                .WithOne();

            builder.Entity<Cart>()
                .HasMany(p => p.Products)
                .WithOne();
        }
    }
}
