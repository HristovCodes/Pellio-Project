namespace Pellio.Data
{
    using Microsoft.EntityFrameworkCore;
    using Pellio.Models;

    /// <summary>
    /// PellioContext class.
    /// </summary>
    public class PellioContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PellioContext" /> class.
        /// </summary>
        /// <param name="options">Options to use for db.</param>
        public PellioContext(DbContextOptions<PellioContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        /// <summary>
        /// Gets or sets Products dbset.
        /// </summary>
        /// <value>Always unique.</value>
        public DbSet<Products> Products { get; set; }

        /// <summary>
        /// Gets or sets Comments dbset.
        /// </summary>
        /// <value>Always unique.</value>
        public DbSet<Comments> Comments { get; set; }

        /// <summary>
        /// Gets or sets OrdersList dbset.
        /// </summary>
        /// <value>Always unique.</value>
        public DbSet<OrdersList> OrdersList { get; set; }

        /// <summary>
        /// Gets or sets EmailCredentials dbset.
        /// </summary>
        /// <value>Always unique.</value>
        public DbSet<EmailCredentials> EmailCredentials { get; set; }

        /// <summary>
        /// Gets or sets MadeOrder dbset.
        /// </summary>
        /// <value>Always unique.</value>
        public DbSet<MadeOrder> MadeOrder { get; set; }

        /// <summary>
        /// Gets or sets PercentOffCodes dbset.
        /// </summary>
        /// <value>Always unique.</value>
        public DbSet<PercentOffCode> PercentOffCodes { get; set; }

        /// <summary>
        /// Executes when model is created.
        /// </summary>
        /// <param name="modelBuilder">This must be something important but idk.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PercentOffCode>()
                .HasOne(a => a.OrdersList)
                .WithOne(b => b.PercentOffCode)
                .HasForeignKey<OrdersList>(b => b.PercentOffCodeId);
        }
    }
}