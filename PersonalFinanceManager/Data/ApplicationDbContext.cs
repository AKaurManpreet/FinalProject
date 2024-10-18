using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Models;

namespace PersonalFinanceManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        // Optional: Configure the precision for the 'Amount' property of the 'Expense' entity here.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Example: Set precision for the 'Amount' property in the 'Expense' entity
            builder.Entity<Expense>()
                .Property(e => e.Amount)
                .HasPrecision(18, 2); // Adjust as needed
        }
    }
}
