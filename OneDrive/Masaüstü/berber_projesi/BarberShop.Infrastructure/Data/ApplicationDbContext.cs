using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BarberShop.Core.Entities;

namespace BarberShop.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Barber> Barbers { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<WorkingHours> WorkingHours { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Appointment configuration
            builder.Entity<Appointment>(entity =>
            {
                entity.HasIndex(a => new { a.BarberId, a.AppointmentDate, a.StartTime })
                    .HasDatabaseName("IX_Appointment_Barber_DateTime");

                entity.HasOne(a => a.User)
                    .WithMany(u => u.Appointments)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Barber)
                    .WithMany(b => b.Appointments)
                    .HasForeignKey(a => a.BarberId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Service)
                    .WithMany(s => s.Appointments)
                    .HasForeignKey(a => a.ServiceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // WorkingHours configuration
            builder.Entity<WorkingHours>(entity =>
            {
                entity.HasIndex(w => new { w.BarberId, w.DayOfWeek })
                    .IsUnique()
                    .HasDatabaseName("IX_WorkingHours_Barber_Day");

                entity.HasOne(w => w.Barber)
                    .WithMany(b => b.WorkingHours)
                    .HasForeignKey(w => w.BarberId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Barber configuration
            builder.Entity<Barber>(entity =>
            {
                entity.HasIndex(b => b.Name)
                    .HasDatabaseName("IX_Barber_Name");
            });

            // Service configuration
            builder.Entity<Service>(entity =>
            {
                entity.HasIndex(s => s.Name)
                    .HasDatabaseName("IX_Service_Name");
            });

            // Announcement configuration
            builder.Entity<Announcement>(entity =>
            {
                entity.HasIndex(a => a.CreatedAt)
                    .HasDatabaseName("IX_Announcement_CreatedAt");
            });
        }
    }
}
