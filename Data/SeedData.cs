using Microsoft.AspNetCore.Identity;
using VolunteerManagementSystem.Models;

namespace VolunteerManagementSystem.Data
{
    public static class SeedData
    {
        public static async Task Initialize(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure database is created
            // For demo/testing we use EnsureCreated to auto-create schema if missing.
            // In production, prefer using EF Core Migrations instead.
            await context.Database.EnsureCreatedAsync();

            // Seed roles if they don't exist
            // The app uses three roles: Superuser (admin), NGO (organizers), Volunteer (participants)
            string[] roleNames = { "Superuser", "NGO", "Volunteer" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed superuser if it doesn't exist
            // This account can log in and manage/approve NGO profiles and oversee the system.
            var superuserEmail = "superadmin@gmail.com";
            var superuser = await userManager.FindByEmailAsync(superuserEmail);
            if (superuser == null)
            {
                superuser = new ApplicationUser
                {
                    UserName = superuserEmail,
                    Email = superuserEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(superuser, "Superadmin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(superuser, "Superuser");
                }
            }

            // Seed sample NGO if it doesn't exist
            // This creates an NGO user and a related NGOProfile for testing the event flow.
            var ngoEmail = "ngo@example.com";
            var ngoUser = await userManager.FindByEmailAsync(ngoEmail);
            if (ngoUser == null)
            {
                ngoUser = new ApplicationUser
                {
                    UserName = ngoEmail,
                    Email = ngoEmail,
                    FirstName = "Community",
                    LastName = "Helpers",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(ngoUser, "NGO123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(ngoUser, "NGO");

                    // Create NGO profile
                    var ngoProfile = new NGOProfile
                    {
                        UserId = ngoUser.Id,
                        Name = "Community Helpers Foundation",
                        Mission = "To serve the community through volunteer work and social initiatives",
                        ContactEmail = ngoEmail,
                        ContactPhone = "+1-555-0123",
                        Address = "123 Community Street, City, State 12345",
                        Description = "A non-profit organization dedicated to making a positive impact in our community through various volunteer programs and social initiatives.",
                        CreatedAt = DateTime.UtcNow
                    };

                    context.NGOProfiles.Add(ngoProfile);
                }
            }

            // Seed sample volunteer if it doesn't exist
            // This user can log in as a volunteer and apply to events.
            var volunteerEmail = "volunteer@example.com";
            var volunteerUser = await userManager.FindByEmailAsync(volunteerEmail);
            if (volunteerUser == null)
            {
                volunteerUser = new ApplicationUser
                {
                    UserName = volunteerEmail,
                    Email = volunteerEmail,
                    FirstName = "John",
                    LastName = "Doe",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(volunteerUser, "Volunteer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(volunteerUser, "Volunteer");
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
