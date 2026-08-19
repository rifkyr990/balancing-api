using BalancingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BalancingApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Planning> Plannings { get; set; }
    public DbSet<PlanningSlot> PlanningSlots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Planning>().HasIndex(x => x.RequestCode).IsUnique();
    }
}