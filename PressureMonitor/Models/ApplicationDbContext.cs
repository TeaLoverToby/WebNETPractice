using Microsoft.EntityFrameworkCore;

namespace PressureMonitor.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<Expense> Expenses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(18, 2);
    }
}