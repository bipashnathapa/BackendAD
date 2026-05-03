using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vehicle.Domain.Models;

namespace Vehicle.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }

    
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<VehicleInfo> Vehicles { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<PartRequest> PartRequests { get; set; }
    public DbSet<ServiceReview> ServiceReviews { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        
        builder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Customer>(c => c.UserID);

        builder.Entity<Staff>()
            .HasOne(s => s.User)
            .WithOne()
            .HasForeignKey<Staff>(s => s.UserID);

        builder.Entity<Appointment>()
            .HasOne(a => a.Customer)
            .WithMany(c => c.Appointments)
            .HasForeignKey(a => a.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Appointment>()
            .HasOne(a => a.Vehicle)
            .WithMany(v => v.Appointments)
            .HasForeignKey(a => a.VehicleID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PartRequest>()
            .HasOne(p => p.Customer)
            .WithMany(c => c.PartRequests)
            .HasForeignKey(p => p.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PartRequest>()
            .HasOne(p => p.Vehicle)
            .WithMany(v => v.PartRequests)
            .HasForeignKey(p => p.VehicleID)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ServiceReview>()
            .HasOne(r => r.Customer)
            .WithMany(c => c.ServiceReviews)
            .HasForeignKey(r => r.CustomerID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ServiceReview>()
            .HasOne(r => r.Appointment)
            .WithMany()
            .HasForeignKey(r => r.AppointmentID)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
