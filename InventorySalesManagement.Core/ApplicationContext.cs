using InventorySalesManagement.Core.Entity.ApplicationData;
using InventorySalesManagement.Core.Entity.OrderData;
using InventorySalesManagement.Core.Entity.SectionsData;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace InventorySalesManagement.Core;

public class ApplicationContext : IdentityDbContext<ApplicationUser>
{
    public virtual DbSet<MainSection> MainSections { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<Order> Orders { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public ApplicationContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ApplicationUser>().ToTable("Users", "dbo");

    }
}
