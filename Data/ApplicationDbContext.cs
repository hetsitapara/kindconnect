using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        // The application's EF Core database context.
        // Inherits ASP.NET Core Identity schema via IdentityDbContext to store users/roles.
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Domain aggregates
        public DbSet<NGOProfile> NGOProfiles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<VolunteerApplication> VolunteerApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure ApplicationUser (additional fields and constraints beyond Identity defaults)
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Configure NGOProfile (one-to-one with ApplicationUser)
            builder.Entity<NGOProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Mission).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(256);
                entity.Property(e => e.ContactPhone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.LogoPath).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.User)
                    .WithOne(e => e.NGOProfile)
                    .HasForeignKey<NGOProfile>(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Event (many-to-one with NGOProfile)
            builder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Venue).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ImagePath).HasMaxLength(200);
                entity.Property(e => e.ContactPerson).HasMaxLength(100);
                entity.Property(e => e.ContactPhone).HasMaxLength(20);
                entity.Property(e => e.ContactEmail).HasMaxLength(256);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.NGO)
                    .WithMany(e => e.Events)
                    .HasForeignKey(e => e.NGOId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure VolunteerApplication (many-to-one to Event and User, with unique constraint)
            builder.Entity<VolunteerApplication>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Message).HasMaxLength(1000);
                entity.Property(e => e.Skills).HasMaxLength(500);
                entity.Property(e => e.Availability).HasMaxLength(500);
                entity.Property(e => e.ResponseMessage).HasMaxLength(500);
                entity.Property(e => e.AppliedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Event)
                    .WithMany(e => e.VolunteerApplications)
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.User)
                    .WithMany(e => e.VolunteerApplications)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.RespondedByUser)
                    .WithMany()
                    .HasForeignKey(e => e.RespondedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Unique constraint to prevent duplicate applications
                entity.HasIndex(e => new { e.EventId, e.UserId }).IsUnique();
            });



            // Seed data for roles so Identity roles exist on first run
            builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().HasData(
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "1", Name = "Superuser", NormalizedName = "SUPERUSER" },
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "2", Name = "NGO", NormalizedName = "NGO" },
                new Microsoft.AspNetCore.Identity.IdentityRole { Id = "3", Name = "Volunteer", NormalizedName = "VOLUNTEER" }
            );
        }
    }
}
